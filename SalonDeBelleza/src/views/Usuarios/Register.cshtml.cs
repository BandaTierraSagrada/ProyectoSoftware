using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalonDeBelleza.src.models;
using SalonDeBelleza.src.services;

namespace SalonDeBelleza.src.views.Usuarios
{
    public class RegisterModel : PageModel
    {
        private readonly UsuarioService _usuarioService;

        public RegisterModel(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [BindProperty]
        public Usuario Usuario { get; set; }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _usuarioService.RegistrarUsuarioAsync(Usuario).Wait();
            return RedirectToPage("/Usuarios/Login");
        }
    }
}