using Microsoft.EntityFrameworkCore;
using SalonDeBelleza.src.config;
using SalonDeBelleza.src.models;
using System.Dynamic;

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
        public async Task<Usuario> CrearColaboradorAsync(Usuario Colaborador,ColaboradorInfo ColaInfo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Guardar usuario primero
                _context.Usuarios.Add(Colaborador);
                await _context.SaveChangesAsync();

                // 2. Asignar el UserID generado al ColaboradorInfo
                ColaInfo.UserID = Colaborador.UserID;
                _context.Colaboradores.Add(ColaInfo);

                // 3. Guardar todo en la base de datos y confirmar la transacción
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Colaborador;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw; // Re-lanzar excepción para manejarla externamente
            }
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
        public async Task ActualizarColaboradorAsync(ColaboradorInfo usuario)
        {
            //var usuarioExistente = await _context.Usuarios.FindAsync(usuario.UserID);
            var usuarioExistente = await _context.Colaboradores.FindAsync(usuario.UserID);
            if (usuarioExistente == null)
            {
                throw new KeyNotFoundException("Usuario no encontrado."); // Lanza una excepción si no se encuentra
            }
            _context.Entry(usuarioExistente).State = EntityState.Detached;
            Console.WriteLine(usuario.HorarioSalida);
            Console.WriteLine(usuarioExistente.HorarioSalida);
            _context.Colaboradores.Update(usuario);
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
        public async Task<ColaboradorInfo> ObtenerPorIdColaborador(int userId)
        {

            return await Task.FromResult(_context.Colaboradores.FirstOrDefault(u => u.UserID == userId));
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
