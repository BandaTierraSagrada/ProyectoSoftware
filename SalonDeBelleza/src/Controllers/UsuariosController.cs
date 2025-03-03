using Microsoft.AspNetCore.Mvc;
using SalonDeBelleza.src.models;
using SalonDeBelleza.src.services;

namespace SalonDeBelleza.src.controllers
{
    [Route("api/usuarios")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public UsuarioController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Usuario usuario)
        {
            var user = await _usuarioService.AutenticarUsuarioAsync(usuario.Email, usuario.Password);
            if (user == null)
            {
                return Unauthorized(new { mensaje = "Credenciales inválidas" });
            }
            return Ok(new { mensaje = "Inicio de sesión exitoso", user });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _usuarioService.CerrarSesion();
            return Ok(new { mensaje = "Sesión cerrada correctamente" });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Usuario usuario)
        {
            var nuevoUsuario = await _usuarioService.RegistrarUsuarioAsync(usuario);
            if (nuevoUsuario == null)
            {
                return BadRequest(new { mensaje = "Error al registrar usuario" });
            }
            return Ok(new { mensaje = "Usuario registrado exitosamente", nuevoUsuario });
        }
    }
}
