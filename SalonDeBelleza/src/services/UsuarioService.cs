using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SalonDeBelleza.src.models;
using SalonDeBelleza.src.repositories;

namespace SalonDeBelleza.src.services
{
    public class UsuarioService
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsuarioService(UsuarioRepository usuarioRepository, IHttpContextAccessor httpContextAccessor)
        {
            _usuarioRepository = usuarioRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Usuario?> AutenticarUsuarioAsync(string email, string password)
        {
            var usuario = await _usuarioRepository.ObtenerPorEmailAsync(email);
            if (usuario == null || usuario.Password != HashPassword(password))
            {
                return null;
            }

            _httpContextAccessor.HttpContext.Session.SetString("UserID", usuario.UserID.ToString());
            _httpContextAccessor.HttpContext.Session.SetString("Rol", usuario.Rol);

            return usuario;
        }
        public async Task ActualizarUsuarioAsync(Usuario usuario)
        {
            await _usuarioRepository.ActualizarUsuarioAsync(usuario);
        }
        public async Task<Usuario> RegistrarUsuarioAsync(Usuario usuario)
        {
            usuario.Password = HashPassword(usuario.Password);
            return await _usuarioRepository.CrearUsuarioAsync(usuario);
        }
        public async Task<Usuario?> ObtenerPorEmailAsync(string email)
        {
            return await _usuarioRepository.ObtenerPorEmailAsync(email);
        }
        public async Task<Usuario> ObtenerPorIdAsync(int id)
        {
            return await _usuarioRepository.ObtenerPorIdAsync(id);
        }
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
