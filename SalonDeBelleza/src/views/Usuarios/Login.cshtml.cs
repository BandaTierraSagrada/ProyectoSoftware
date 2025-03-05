using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalonDeBelleza.src.services;

namespace SalonDeBelleza.src.views.Usuarios
{
    public class LoginModel : PageModel
    {
        private readonly UsuarioService _usuarioService;

        public LoginModel(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string Mensaje { get; set; }

        public void OnGet() { }
        public async Task<IActionResult> OnPost()
        {
            var usuario = await _usuarioService.AutenticarUsuarioAsync(Email, Password);
            if (usuario == null)
            {
                Mensaje = "Correo o contraseña incorrectos.";
                return Page();
            }

            // Guardar sesión
            HttpContext.Session.SetInt32("UserID", usuario.UserID);
            HttpContext.Session.SetString("Nombre", usuario.Nombre);
            HttpContext.Session.SetString("Rol", usuario.Rol);

            return RedirectToPage("/Home/"+usuario.Rol);
        }

    }
}
