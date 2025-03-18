using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalonDeBelleza.src.models;
using SalonDeBelleza.src.services;

namespace SalonDeBelleza.src.views.Administrador
{
    public class CrearUsuarioModel : PageModel
    {
        private readonly UsuarioService _usuarioService;

        public CrearUsuarioModel(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [BindProperty]
        public Usuario Colaborador { get; set; }
        [BindProperty]
        public TimeSpan HorarioEntrada { get; set; }
        [BindProperty]
        public TimeSpan HorarioSalida { get; set; }
        [BindProperty]
        public string TipoServicio { get; set; }
        [BindProperty]
        public int DuracionServicio { get; set; }
        public string Mensaje { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var existeUsuario = await _usuarioService.ObtenerPorEmailAsync(Colaborador.Email);
            if (existeUsuario != null)
            {
                Mensaje = "El correo ya está registrado.";
                return Page();
            }
            if (HorarioEntrada >= HorarioSalida)
            {
                Mensaje = "El horario de entrada debe ser antes que el de salida.";
                return Page();
            }
            ColaboradorInfo ColaInfo = new ColaboradorInfo
            {
                HorarioEntrada = HorarioEntrada,
                HorarioSalida = HorarioSalida,
                TipoServicio = TipoServicio,
                DuracionServicio = DuracionServicio
            };
            await _usuarioService.RegistrarColaboradorAsync(Colaborador,ColaInfo);
            return RedirectToPage("/Administrador/AdminUsuarios");
        }
    }
}
