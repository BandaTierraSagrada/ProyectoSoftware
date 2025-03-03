using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using SalonDeBelleza.src.models;
using SalonDeBelleza.src.services;

namespace SalonDeBelleza.src.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public UsuarioController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // Obtener todos los usuarios
        [HttpGet]
        public async Task<ActionResult<List<Usuario>>> GetAllUsuarios()
        {
            return await _usuarioService.GetAllUsuariosAsync();
        }

        // Obtener un usuario por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuarioById(int id)
        {
            var usuario = await _usuarioService.GetUsuarioByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return usuario;
        }

        // Crear un nuevo usuario
        [HttpPost]
        public async Task<ActionResult> CreateUsuario(Usuario usuario)
        {
            await _usuarioService.CreateUsuarioAsync(usuario);
            return CreatedAtAction(nameof(GetUsuarioById), new { id = usuario.UserID }, usuario);
        }

        // Actualizar un usuario existente
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUsuario(int id, Usuario usuario)
        {
            if (id != usuario.UserID)
            {
                return BadRequest();
            }
            await _usuarioService.UpdateUsuarioAsync(usuario);
            return NoContent();
        }

        // Eliminar un usuario por ID
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUsuario(int id)
        {
            await _usuarioService.DeleteUsuarioAsync(id);
            return NoContent();
        }

        // Autenticar usuario (login)
        [HttpPost("login/email")]
        public async Task<ActionResult<Usuario>> LoginByEmail(string email, string password)
        {
            var usuario = await _usuarioService.AuthenticateByEmailAsync(email, password);
            if (usuario == null)
            {
                return Unauthorized();
            }
            return usuario;
        }

        // Autenticar por teléfono y contraseña
        [HttpPost("login/phone")]
        public async Task<ActionResult<Usuario>> LoginByPhone(string telefono, string password)
        {
            var usuario = await _usuarioService.AuthenticateByPhoneAsync(telefono, password);
            if (usuario == null)
            {
                return Unauthorized();
            }
            return usuario;
        }
    }
}