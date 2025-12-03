using APIUsuarios.Application.Interfaces;
using APIUsuarios.Domain.Entities;
using APIUsuarios.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace APIUsuarios.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Usuario>> GetAllAsync(CancellationToken ct)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .Where(u => u.Ativo)
            .ToListAsync(ct);
    }

    public async Task<Usuario?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);
    }

    public async Task AddAsync(Usuario usuario, CancellationToken ct)
    {
        await _context.Usuarios.AddAsync(usuario, ct);
    }

    public async Task UpdateAsync(Usuario usuario, CancellationToken ct)
    {
        _context.Usuarios.Update(usuario);
        await Task.CompletedTask;
    }

    public async Task RemoveAsync(Usuario usuario, CancellationToken ct)
    {
        _context.Usuarios.Remove(usuario);
        await Task.CompletedTask;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct)
    {
        return await _context.Usuarios
            .AnyAsync(u => u.Email == email.ToLowerInvariant(), ct);
    }

    public async Task<bool> EmailExistsForOtherUserAsync(int id, string email, CancellationToken ct)
    {
        return await _context.Usuarios
            .AnyAsync(u => u.Email == email.ToLowerInvariant() && u.Id != id, ct);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return await _context.SaveChangesAsync(ct);
    }
}