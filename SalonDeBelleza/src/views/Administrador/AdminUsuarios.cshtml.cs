using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalonDeBelleza.src.services;
using SalonDeBelleza.src.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SalonDeBelleza.src.views.Administrador
{
    public class AdminUsuariosModel : PageModel
    {
        private readonly UsuarioService _usuarioService;

        public AdminUsuariosModel(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public List<Usuario> Usuarios { get; set; } = new List<Usuario>();

        [BindProperty]
        public Usuario UsuarioEdit { get; set; }
        [BindProperty]
        public ColaboradorInfo? ColaboradorEdit { get; set; }

        [BindProperty]
        public string? NuevaContrasena { get; set; } = "";

        [BindProperty]
        public string? ConfirmarContrasena { get; set; } = "";
        [BindProperty]
        public int UserIDContra { get; set; }
        [BindProperty]
        public string decision { get; set; }


        public async Task<IActionResult> OnGet(int UserID)
        {
            if (UserID != 0)
            {
                UsuarioEdit = await _usuarioService.ObtenerPorIdAdmin(UserID);
            }
            Usuarios = await _usuarioService.ObtenerTodosAsync();
            return Page();
        }

        public async Task<IActionResult> OnGetEditarAsync(int UserID)
        {
            var usuario = await _usuarioService.ObtenerPorIdAdmin(UserID);
            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuario no encontrado.");
                return Page();
            }
            this.decision = decision;
            UsuarioEdit = await _usuarioService.ObtenerPorIdAdmin(UserID);
            if(UsuarioEdit.Rol == "Colaborador")
            {
                ColaboradorEdit = await _usuarioService.ObtenerPorIdColaborador(UserID);
            }

            Usuarios = await _usuarioService.ObtenerTodosAsync();
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {

            if (decision == "ActualizarUsuario")
            {
                return await HandleActualizar();
            }
            else if (decision == "CambiarContrasena")
            {
                return await HandleCambiarContrasena();
            }
            else if(decision == "EliminarUsuario")
            {
                return await HandleEliminarUsuario();
            }

            return Page();
        }
        private async Task<IActionResult> HandleActualizar()
        {
            var usuario = await _usuarioService.ObtenerPorIdAdmin(UsuarioEdit.UserID);
            UsuarioEdit.Password = usuario.Password;
            Console.WriteLine("OnPost Actualizar ejecutado");
            

            Console.WriteLine("OnPost Actualizar actualizando usuario");
            Console.WriteLine($" Datos recibidos: ID={UsuarioEdit.UserID}, Nombre={UsuarioEdit.Nombre}, Email={UsuarioEdit.Email}");
            Console.WriteLine($" Datos recibidos: UserID={ColaboradorEdit.UserID}, HorarioEntrada={ColaboradorEdit.HorarioEntrada}, HorarioSalida={ColaboradorEdit.HorarioSalida}, TipoServicio={ColaboradorEdit.TipoServicio},DuracionServicio={ColaboradorEdit.DuracionServicio}");

            await _usuarioService.ActualizarUsuarioAsync(UsuarioEdit);
            if (UsuarioEdit.Rol == "Colaborador")
            {
                await _usuarioService.ActualizarColaboradorAsync(ColaboradorEdit);
            }
            return Page();
        }

        private async Task<IActionResult> HandleCambiarContrasena()
        {
            var usuario = await _usuarioService.ObtenerPorIdAdmin(UsuarioEdit.UserID);
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($" Error en {state.Key}: {error.ErrorMessage}");
                }
            }
            if (string.IsNullOrWhiteSpace(NuevaContrasena) || NuevaContrasena != ConfirmarContrasena)
            {
                ModelState.AddModelError("", "Las contraseñas no coinciden.");
                Usuarios = await _usuarioService.ObtenerTodosAsync();
                return Page();
            }
            Console.WriteLine($" Datos recibidos: ID={UsuarioEdit.UserID}, Nombre={UsuarioEdit.Nombre}, Email={UsuarioEdit.Email}");
            await _usuarioService.CambiarContrasenaAsync(usuario.UserID, NuevaContrasena);
            return RedirectToPage();
        }

        private async Task<IActionResult> HandleEliminarUsuario()
        {
            int userId = Int32.Parse(Request.Form["userId"]);
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($" Error en {state.Key}: {error.ErrorMessage}");
                }
            }
            await _usuarioService.EliminarUsuarioAsync(userId);
            return RedirectToPage();
        }
    }
}
