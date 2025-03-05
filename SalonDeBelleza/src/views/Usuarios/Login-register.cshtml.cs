using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalonDeBelleza.src.services;
using SalonDeBelleza.src.models;

namespace SalonDeBelleza.src.views.Usuarios
{
    public class Login_registerModel : PageModel
    {
        private readonly UsuarioService _usuarioService;

        public Login_registerModel(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [BindProperty]
        public Usuario Usuario { get; set; }
        [BindProperty]
        public string LEmail { get; set; }

        [BindProperty]
        public string LPassword { get; set; }
        public string Mensaje { get; set; }

        public void OnGet() { }
        public async Task<IActionResult> OnPost()
        {
            var decision = Request.Form["decision"];

            if (decision == "Register")
            {
                return await HandleRegistration();
            }
            else if (decision == "Login")
            {
                return await HandleLogin();
            }

            return Page();
        }
        private async Task<IActionResult> HandleRegistration()
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
        private async Task<IActionResult> HandleLogin()
        {
            var usuario = await _usuarioService.AutenticarUsuarioAsync(LEmail, LPassword);
            if (usuario == null)
            {
                Mensaje = "Correo o contraseña incorrectos.";
                return Page();
            }

            // Guardar sesión
            HttpContext.Session.SetInt32("UserID", usuario.UserID);
            HttpContext.Session.SetString("Nombre", usuario.Nombre);
            HttpContext.Session.SetString("Rol", usuario.Rol);

            return RedirectToPage("/Home/" + usuario.Rol);
        }
    }
}
