using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalonDeBelleza.src.models;
using SalonDeBelleza.src.services;
namespace SalonDeBelleza.src.views.Citas
{
    public class CitasColaboradorModel : PageModel
    {

        public int? UserID { get; set; }
        public string? Nombre { get; set; }
        public string? Rol { get; set; }
        public void OnGet()
        {
            UserID = HttpContext.Session.GetInt32("UserID");
            Rol = HttpContext.Session.GetString("Rol") ?? "Cliente";
            Nombre = HttpContext.Session.GetString("Nombre") ?? "Invitado";

        }


    }
}
