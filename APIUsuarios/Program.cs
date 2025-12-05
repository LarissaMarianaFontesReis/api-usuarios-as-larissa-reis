using APIUsuarios.Application.Interfaces;
using APIUsuarios.Application.Services;
using APIUsuarios.Application.Validators;
using APIUsuarios.Infrastructure.Persistence;
using APIUsuarios.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using APIUsuarios.Application.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registro de dependências
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// Validações
builder.Services.AddValidatorsFromAssemblyContaining<UsuarioCreateDtoValidator>();

// Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Endpoints da API
var usuarios = app.MapGroup("/usuarios");

usuarios.MapGet("/", async (IUsuarioService service, CancellationToken ct) =>
{
    var usuarios = await service.ListarAsync(ct);
    return Results.Ok(usuarios);
})
.WithName("ListarUsuarios")
.WithOpenApi();

usuarios.MapGet("/{id:int}", async (int id, IUsuarioService service, CancellationToken ct) =>
{
    var usuario = await service.ObterAsync(id, ct);
    return usuario is not null ? Results.Ok(usuario) : Results.NotFound();
})
.WithName("ObterUsuario")
.WithOpenApi();

usuarios.MapPost("/", async (UsuarioCreateDto dto, IUsuarioService service, IValidator<UsuarioCreateDto> validator, CancellationToken ct) =>
{
    var validationResult = await validator.ValidateAsync(dto, ct);
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

    try
    {
        var usuario = await service.CriarAsync(dto, ct);
        return Results.Created($"/usuarios/{usuario.Id}", usuario);
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new { error = ex.Message });
    }
})
.WithName("CriarUsuario")
.WithOpenApi();

usuarios.MapPut("/{id:int}", async (int id, UsuarioUpdateDto dto, IUsuarioService service, IValidator<UsuarioUpdateDto> validator, CancellationToken ct) =>
{
    var validationResult = await validator.ValidateAsync(dto, ct);
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

    try
    {
        var usuario = await service.AtualizarAsync(id, dto, ct);
        return Results.Ok(usuario);
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound();
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new { error = ex.Message });
    }
})
.WithName("AtualizarUsuario")
.WithOpenApi();

usuarios.MapDelete("/{id:int}", async (int id, IUsuarioService service, CancellationToken ct) =>
{
    var removido = await service.RemoverAsync(id, ct);
    return removido ? Results.NoContent() : Results.NotFound();
})
.WithName("RemoverUsuario")
.WithOpenApi();

// Criar banco de dados e aplicar migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();