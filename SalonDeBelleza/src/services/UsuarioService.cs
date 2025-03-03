using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalonDeBelleza.src.models;
using SalonDeBelleza.src.repositories;

namespace SalonDeBelleza.src.services
{
    public class UsuarioService
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly ApplicationDbContext _context;

        public UsuarioService(ApplicationDbContext context)
        {
            _context = context;
        }
        public UsuarioService(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        // Obtener todos los usuarios
        public async Task<List<Usuario>> GetAllUsuariosAsync()
        {
            return await _usuarioRepository.GetAllUsuariosAsync();
        }

        // Obtener un usuario por ID
        public async Task<Usuario> GetUsuarioByIdAsync(int id)
        {
            return await _usuarioRepository.GetUsuarioByIdAsync(id);
        }

        // Crear un nuevo usuario
        public async Task<bool> RegistrarUsuarioAsync(Usuario usuario)
        {
            // Asignar el rol "Cliente" por defecto
            usuario.Rol = "Cliente";
            usuario.Loginstatus = "Inactivo";

            // Hash de la contraseña
            usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        // Actualizar un usuario existente
        public async Task UpdateUsuarioAsync(Usuario usuario)
        {
            // Si se actualiza la contraseña, hashearla
            if (!string.IsNullOrEmpty(usuario.Password))
            {
                usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
            }
            await _usuarioRepository.UpdateUsuarioAsync(usuario);
        }

        // Eliminar un usuario por ID
        public async Task DeleteUsuarioAsync(int id)
        {
            await _usuarioRepository.DeleteUsuarioAsync(id);
        }

        // Autenticar usuario (login)
        public async Task<Usuario> AutenticarUsuarioAsync(string emailOrPhone, string password)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == emailOrPhone || u.Telefono == emailOrPhone);

            if (usuario != null && BCrypt.Net.BCrypt.Verify(password, usuario.Password))
            {
                return usuario;
            }

            return null;
        }
    }
}