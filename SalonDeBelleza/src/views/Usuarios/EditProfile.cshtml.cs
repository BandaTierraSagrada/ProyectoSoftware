using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalonDeBelleza.src.services;
using SalonDeBelleza.src.models;
using System.Threading.Tasks;

namespace SalonDeBelleza.src.views.Usuarios
{
    public class EditProfileModel : PageModel
    {
        private readonly UsuarioService _usuarioService;

        public EditProfileModel(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [BindProperty]
        public Usuario Usuario { get; set; }

        [BindProperty]
        public string NuevaPassword { get; set; }

        [BindProperty]
        public string ConfirmarPassword { get; set; }

        public string Mensaje { get; set; }
        public int? UserID { get; set; }
        public string Rol { get; private set; }

        public async Task<IActionResult> OnGet()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            UserID = userId;
            Rol = HttpContext.Session.GetString("Rol") ?? "Cliente";
            if (userId == null)
            {
                return RedirectToPage("/Usuarios/Login-register");
            }

            Usuario = await _usuarioService.ObtenerPorIdAsync(userId.Value);
            if (Usuario == null)
            {
                return RedirectToPage("/Usuarios/Login-register");
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            UserID = userId;
            if (userId == null)
            {
                return RedirectToPage("/Usuarios/Login-register");
            }

            var usuarioExistente = await _usuarioService.ObtenerPorIdAsync(userId.Value);
            if (usuarioExistente == null)
            {
                return RedirectToPage("/Usuarios/Login-register");
            }

            // Validar que las contraseñas coincidan
            if (!string.IsNullOrEmpty(NuevaPassword) && NuevaPassword != ConfirmarPassword)
            {
                Mensaje = "Las contraseñas no coinciden.";
                return Page();
            }

            // Actualizar los datos del usuario
            usuarioExistente.Nombre = Usuario.Nombre;
            usuarioExistente.Telefono = Usuario.Telefono;
            usuarioExistente.Email = Usuario.Email;

            if (!string.IsNullOrEmpty(NuevaPassword))
            {
                usuarioExistente.Password = _usuarioService.HashPassword(NuevaPassword);
            }

            try
            {
                await _usuarioService.ActualizarUsuarioAsync(usuarioExistente);
                Mensaje = "Perfil actualizado exitosamente.";
            }
            catch (Exception)
            {
                Mensaje = "Error al actualizar el perfil.";
            }
            return Page();
        }
    }
}
