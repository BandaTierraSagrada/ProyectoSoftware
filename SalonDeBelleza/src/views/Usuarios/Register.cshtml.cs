using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalonDeBelleza.src.services;
using SalonDeBelleza.src.models;

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
        public string Mensaje { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var existeUsuario = await _usuarioService.ObtenerPorEmailAsync(Usuario.Email);
            if (existeUsuario != null)
            {
                Mensaje = "El correo ya está registrado.";
                return Page();
            }

            await _usuarioService.RegistrarUsuarioAsync(Usuario);
            return RedirectToPage("/Usuarios/Login");
        }
    }
}

