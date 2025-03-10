using Microsoft.EntityFrameworkCore;
using SalonDeBelleza.src.config;
using SalonDeBelleza.src.models;

namespace SalonDeBelleza.src.repositories
{
    public class UsuarioRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObtenerPorEmailAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Usuario> CrearUsuarioAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }
        public async Task ActualizarUsuarioAsync(Usuario usuario)
        {
            var usuarioExistente = await _context.Usuarios.FindAsync(usuario.UserID);
            if (usuarioExistente == null)
            {
                throw new KeyNotFoundException("Usuario no encontrado."); // Lanza una excepción si no se encuentra
            }
            _context.Entry(usuarioExistente).State = EntityState.Detached;
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }
        public async Task ActualizarUsuarioContrasena(Usuario usuario)
        {
            var usuarioExistente = await _context.Usuarios.FindAsync(usuario.UserID);
            if (usuarioExistente == null)
            {
                throw new KeyNotFoundException("Usuario no encontrado."); // Lanza una excepción si no se encuentra
            }
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<Usuario> ObtenerPorIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }
        public async Task<Usuario> ObtenerPorIdAdmin(int userId)
        {
            
            return await Task.FromResult(_context.Usuarios.FirstOrDefault(u => u.UserID == userId));
        }
        public async Task<List<Usuario>> ObtenerTodosAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task EliminarUsuarioAsync(int userId)
        {
            var usuario = await ObtenerPorIdAsync(userId);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }
    }
}
