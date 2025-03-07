using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using SalonDeBelleza.src.services;

namespace SalonDeBelleza.src.views.Usuarios
{
    public class LogoutModel : PageModel
    {
        private readonly UsuarioService _usuarioService;

        public LogoutModel(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public async Task<IActionResult> OnGet()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToPage("/Usuarios/Login-register");
            }

            var usuario = await _usuarioService.ObtenerPorIdAsync(userId.Value);
            if (usuario != null)
            {
                usuario.LoginStatus = "Inactivo";
                await _usuarioService.ActualizarUsuarioAsync(usuario);
            }

            // Cerrar sesión
            HttpContext.Session.Clear();

            return RedirectToPage("/Usuarios/Login-register");
        }
    }
}
