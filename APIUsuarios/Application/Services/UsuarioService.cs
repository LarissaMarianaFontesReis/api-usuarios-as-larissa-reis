using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Domain.Entities;

namespace APIUsuarios.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly ILogger<UsuarioService> _logger;

    public UsuarioService(IUsuarioRepository repository, ILogger<UsuarioService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<UsuarioReadDto>> ListarAsync(CancellationToken ct)
    {
        var usuarios = await _repository.GetAllAsync(ct);
        return usuarios.Select(u => new UsuarioReadDto(
            u.Id,
            u.Nome,
            u.Email,
            u.DataNascimento,
            u.Telefone,
            u.Ativo,
            u.DataCriacao
        ));
    }

    public async Task<UsuarioReadDto?> ObterAsync(int id, CancellationToken ct)
    {
        var usuario = await _repository.GetByIdAsync(id, ct);
        if (usuario == null) return null;

        return new UsuarioReadDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.DataNascimento,
            usuario.Telefone,
            usuario.Ativo,
            usuario.DataCriacao
        );
    }

    public async Task<UsuarioReadDto> CriarAsync(UsuarioCreateDto dto, CancellationToken ct)
    {
        // Normalizar email para lowercase
        var emailNormalizado = dto.Email.ToLowerInvariant();
        
        // Verificar se email já existe
        if (await _repository.EmailExistsAsync(emailNormalizado, ct))
        {
            throw new InvalidOperationException($"Email '{dto.Email}' já está cadastrado.");
        }

        // Verificar idade mínima
        var idade = DateTime.Now.Year - dto.DataNascimento.Year;
        if (dto.DataNascimento.Date > DateTime.Now.AddYears(-18))
        {
            throw new InvalidOperationException("Usuário deve ter pelo menos 18 anos.");
        }

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = emailNormalizado,
            Senha = dto.Senha, // Em produção, hash da senha
            DataNascimento = dto.DataNascimento.Date,
            Telefone = dto.Telefone,
            Ativo = true,
            DataCriacao = DateTime.UtcNow
        };

        await _repository.AddAsync(usuario, ct);
        await _repository.SaveChangesAsync(ct);

        return new UsuarioReadDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.DataNascimento,
            usuario.Telefone,
            usuario.Ativo,
            usuario.DataCriacao
        );
    }

    public async Task<UsuarioReadDto> AtualizarAsync(int id, UsuarioUpdateDto dto, CancellationToken ct)
    {
        var usuario = await _repository.GetByIdAsync(id, ct);
        if (usuario == null)
        {
            throw new KeyNotFoundException($"Usuário com ID {id} não encontrado.");
        }

        // Verificar se email já existe para outro usuário
        if (await _repository.EmailExistsForOtherUserAsync(id, dto.Email.ToLowerInvariant(), ct))
        {
            throw new InvalidOperationException($"Email '{dto.Email}' já está cadastrado para outro usuário.");
        }

        // Verificar idade mínima
        var idade = DateTime.Now.Year - dto.DataNascimento.Year;
        if (dto.DataNascimento.Date > DateTime.Now.AddYears(-18))
        {
            throw new InvalidOperationException("Usuário deve ter pelo menos 18 anos.");
        }

        usuario.Nome = dto.Nome;
        usuario.Email = dto.Email.ToLowerInvariant();
        usuario.DataNascimento = dto.DataNascimento.Date;
        usuario.Telefone = dto.Telefone;
        usuario.Ativo = dto.Ativo;
        usuario.DataAtualizacao = DateTime.UtcNow;

        await _repository.UpdateAsync(usuario, ct);
        await _repository.SaveChangesAsync(ct);

        return new UsuarioReadDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.DataNascimento,
            usuario.Telefone,
            usuario.Ativo,
            usuario.DataCriacao
        );
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken ct)
    {
        var usuario = await _repository.GetByIdAsync(id, ct);
        if (usuario == null) return false;

        usuario.Ativo = false;
        usuario.DataAtualizacao = DateTime.UtcNow;

        await _repository.UpdateAsync(usuario, ct);
        return await _repository.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> EmailJaCadastradoAsync(string email, CancellationToken ct)
    {
        return await _repository.EmailExistsAsync(email.ToLowerInvariant(), ct);
    }
}