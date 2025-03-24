using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SalonDeBelleza.src.views.Citas
{
    public class CitasClienteModel : PageModel
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
