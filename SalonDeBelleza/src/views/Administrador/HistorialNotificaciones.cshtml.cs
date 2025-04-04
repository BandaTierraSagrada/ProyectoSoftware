using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SalonDeBelleza.src.config;
using SalonDeBelleza.src.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace SalonDeBelleza.src.views.Administrador
{
    public class HistorialNotificacionesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public HistorialNotificacionesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();
        public DateTime? FechaFiltro { get; set; }
        public string TipoFiltro { get; set; }

        public async Task OnGetAsync(DateTime? fecha, string tipo)
        {
            var query = _context.Notificaciones.AsQueryable();

            if (fecha.HasValue)
            {
                query = query.Where(n => n.FechaEnvio.Date == fecha.Value.Date);
                FechaFiltro = fecha;
            }

            if (!string.IsNullOrEmpty(tipo))
            {
                query = query.Where(n => n.Tipo == tipo);
                TipoFiltro = tipo;
            }

            Notificaciones = await query
                .OrderByDescending(n => n.FechaEnvio)
                .ToListAsync();
        }
    }
}
