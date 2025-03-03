using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace SalonDeBelleza.src.views.Home
{
    public class AdministradorModel : PageModel
    {
        public string UserName { get; private set; }

        public void OnGet()
        {
            UserName = HttpContext.Session.GetString("UserName") ?? "Invitado";
        }
    }
}
