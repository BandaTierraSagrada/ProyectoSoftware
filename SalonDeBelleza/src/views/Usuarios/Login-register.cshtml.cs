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

        public class InputModel
        {
            // Login
            public string? LEmail { get; set; }
            public string? LPassword { get; set; }

            // Registro
            public string? Nombre { get; set; }
            public string? Telefono { get; set; }
            public string? Email { get; set; }
            public string? Password { get; set; }
            public string Rol { get; set; } = "Cliente";
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();
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
            if (string.IsNullOrEmpty(Input.Nombre) ||
                string.IsNullOrEmpty(Input.Telefono) ||
                string.IsNullOrEmpty(Input.Email) ||
                string.IsNullOrEmpty(Input.Password))
            {
                Mensaje = "Todos los campos son obligatorios.";
                return Page();
            }

            var existeUsuario = await _usuarioService.ObtenerPorEmailAsync(Input.Email);
            if (existeUsuario != null)
            {
                Mensaje = "El correo ya está registrado.";
                return Page();
            }

            var nuevoUsuario = new Usuario
            {
                Nombre = Input.Nombre,
                Telefono = Input.Telefono,
                Email = Input.Email,
                Password = Input.Password,
                Rol = Input.Rol
            };
            await _usuarioService.RegistrarUsuarioAsync(nuevoUsuario);
            Mensaje = "Registrado correctamente";
            return RedirectToPage("/Usuarios/Login-register");
        }
        private async Task<IActionResult> HandleLogin()
        {
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    Console.WriteLine("OnPost Actualizar invalido");

                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Error en {key}; {error.ErrorMessage}");
                    }
                }
            }
                var usuario = await _usuarioService.AutenticarUsuarioAsync(Input.LEmail,Input.LPassword);

            if (usuario == null)
            {
                Mensaje = "Correo o contrasena incorrectos.";
                return Page();
            }
            HttpContext.Session.SetInt32("UserID", usuario.UserID);
            HttpContext.Session.SetString("Nombre", usuario.Nombre);
            HttpContext.Session.SetString("Rol", usuario.Rol);
            usuario.LoginStatus = "Activo";
            await _usuarioService.ActualizarUsuarioAsync(usuario);

            return RedirectToPage("/Home/" + usuario.Rol);
        }
    }
}
