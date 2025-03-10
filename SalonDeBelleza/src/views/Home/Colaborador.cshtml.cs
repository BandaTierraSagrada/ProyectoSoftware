using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace SalonDeBelleza.src.views.Home
{
    public class ColaboradorModel : PageModel
    {
        public string Nombre { get; private set; }
        public void OnGet()
        {
            Nombre = HttpContext.Session.GetString("Nombre") ?? "Invitado";
        }
    }
}
