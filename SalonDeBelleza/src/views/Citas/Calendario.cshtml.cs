using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SalonDeBelleza.src.config;
using SalonDeBelleza.src.models;
using SalonDeBelleza.src.services;
namespace SalonDeBelleza.src.Citas
{
    public class CalendarioModel : PageModel
    {
        private readonly UsuarioService _usuarioService;
        private readonly CitaService _citaService;

        public CalendarioModel(UsuarioService usuarioService, CitaService citaService)
        {
            _usuarioService = usuarioService;
            _citaService = citaService;
        }

        public string Nombre { get; private set; }
        public int? UserID { get; private set; }
        public int? UserIDCliente { get; set; }

        public string Rol { get; private set; }

        [BindProperty]
        public string EmailBusqueda { get; set; }
        public Usuario UsuarioEncontrado { get; private set; }

        public List<string> ServiciosD { get; set; } = new List<string> { "Corte", "Tintes", "Manicura", "Pedicura" };

        public void OnGet()
        {
            UserID = HttpContext.Session.GetInt32("UserID");
            UserIDCliente = HttpContext.Session.GetInt32("UserID");
            Nombre = HttpContext.Session.GetString("Nombre") ?? "Invitado";
            Rol = HttpContext.Session.GetString("Rol") ?? "Cliente";

        }

        


    }
}
