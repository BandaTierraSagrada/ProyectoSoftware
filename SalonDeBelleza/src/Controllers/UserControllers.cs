using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalonDeBelleza.src.models;
using System.Linq;
using System.Threading.Tasks;

namespace SalonDeBelleza.src.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Autenticar un usuario
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);

            if (usuario == null)
            {
                return Unauthorized("Credenciales inválidas.");
            }

            // Actualizar el estado de inicio de sesión
            usuario.Loginstatus = "Activo";
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Autenticación exitosa.", Usuario = usuario });
        }

        // Crear un nuevo usuario
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] Usuario usuario)
        {
            if (usuario == null)
            {
                return BadRequest("El usuario no puede ser nulo.");
            }

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserById), new { id = usuario.UserID }, usuario);
        }

        // Obtener un usuario por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            return Ok(usuario);
        }

        // Modificar un usuario
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] Usuario usuario)
        {
            if (id != usuario.UserID)
            {
                return BadRequest("ID de usuario no coincide.");
            }

            var existingUser = await _context.Usuarios.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            existingUser.Nombre = usuario.Nombre;
            existingUser.Telefono = usuario.Telefono;
            existingUser.Email = usuario.Email;
            existingUser.Password = usuario.Password;
            existingUser.Rol = usuario.Rol;
            existingUser.UpdatedAt = DateTime.UtcNow;

            _context.Usuarios.Update(existingUser);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Usuario actualizado correctamente.", Usuario = existingUser });
        }

        // Eliminar un usuario
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Usuario eliminado correctamente." });
        }
    }

    // Modelo para la solicitud de autenticación
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}