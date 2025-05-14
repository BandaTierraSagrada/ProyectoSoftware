using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using SalonDeBelleza.src.config;
using SalonDeBelleza.src.models;

namespace SalonDeBelleza.src.views.Home
{
    public class ColaboradorModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ColaboradorModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public string Nombre { get; private set; }
        public int UserID { get; set; }
        public List<(DateTime FechaHora, string Cliente, string Colaborador)> CitasProximas { get; set; }
        public int TotalCitas { get; set; }
        public int ClientesAtendidos { get; set; }
        public string TiempoEstimado { get; set; }
        public async Task OnGet()
        {
            Nombre = HttpContext.Session.GetString("Nombre") ?? "Invitado";
            UserID = (int)HttpContext.Session.GetInt32("UserID");
            if (UserID == 0) {
                Response.Redirect(Url.Page("/Usuarios/Login-register"));
            }

            var ahora = DateTime.Now;
            var hoy = ahora.Date;

            // Citas próximas (las siguientes 5)
            CitasProximas = await _context.Citas
                .Where(c => c.ColaboradorID == UserID && c.FechaHora >= ahora)
                .OrderBy(c => c.FechaHora)
                .Take(5)
                .Select(c => new
                {
                    c.FechaHora,
                    Cliente = c.Cliente.Nombre,
                    Colaborador = c.Colaborador.Nombre
                })
                .AsNoTracking()
                .ToListAsync()
                .ContinueWith(t => t.Result
                .Select(x => (x.FechaHora, x.Cliente, x.Colaborador)).ToList());

            // Total de citas del día
            TotalCitas = await _context.Citas
                .Where(c => c.ColaboradorID == UserID && c.FechaHora.Date == hoy)
                .CountAsync();
        }
    }
}
