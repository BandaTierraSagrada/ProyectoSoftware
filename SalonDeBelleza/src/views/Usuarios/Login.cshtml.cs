using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalonDeBelleza.src.models;
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
        public string EmailOrPhone { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public IActionResult OnPost()
        {
            var usuario = _usuarioService.AutenticarUsuarioAsync(EmailOrPhone, Password).Result;

            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
                return Page();
            }

            // Guardar el usuario en la sesión
            HttpContext.Session.SetString("Nombre", usuario.Nombre);
            HttpContext.Session.SetString("Rol", usuario.Rol);

            // Redirigir según el rol
            return usuario.Rol switch
            {
                "Cliente" => RedirectToPage("/Home/Cliente"),
                "Colaborador" => RedirectToPage("/Home/Colaborador"),
                "Administrador" => RedirectToPage("/Home/Administrador"),
                _ => RedirectToPage("/Index")
            };
        }
    }
}