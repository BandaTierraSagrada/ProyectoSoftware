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

        public IActionResult OnGet()
        {
            _usuarioService.CerrarSesion();
            return RedirectToPage("/Usuarios/Login");
        }
    }
}
