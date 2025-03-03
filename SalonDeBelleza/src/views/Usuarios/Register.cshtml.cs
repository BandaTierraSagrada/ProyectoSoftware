using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalonDeBelleza.src.models;
using System.Threading.Tasks;

namespace SalonDeBelleza.src.views.Usuarios
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<Usuario> _userManager;

        public RegisterModel(UserManager<Usuario> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public Usuario Usuario { get; set; } = new Usuario();

        [BindProperty]
        public string Password { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var errors = ModelState.Where(x => x.Value.Errors.Any())
                .Select(x => new { x.Key, x.Value.Errors });
            Console.WriteLine(errors.ToString());
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Console.WriteLine("aqui");
            // Crear un nuevo usuario a partir de los datos del formulario
            var usuario = new Usuario
            {
                Nombre = Usuario.Nombre,
                Telefono = Usuario.Telefono,
                Email = Usuario.Email,
                UserName = Usuario.Email // Usar el email como nombre de usuario
            };
            Console.WriteLine("aqui");
            var result = await _userManager.CreateAsync(usuario, Password);
            if (result.Succeeded)
            {
                return RedirectToPage("/Usuarios/Login"); // Redirigir a la página de inicio de sesión
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}