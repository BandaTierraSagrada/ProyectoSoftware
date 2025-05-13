using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SalonDeBelleza.src.models;
using SalonDeBelleza.src.config;
namespace SalonDeBelleza.src.views.Home
{
    public class AdministradorModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AdministradorModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public string Nombre { get; private set; }
        public List<(DateTime FechaHora, string Cliente, string Colaborador)> CitasProximas { get; set; }
        public List<(string Nombre, int Stock, int StockMinimo)> ProductosConStockBajo { get; set; }
        public Dictionary<string, int> CitasPorColaborador { get; set; }
        public List<string> NombresColaboradores { get; set; } = new();
        public List<int> CantidadCitas { get; set; } = new();
        public async Task OnGetAsync()
        {
            Nombre = HttpContext.Session.GetString("Nombre") ?? "Invitado";

            CitasProximas = await _context.Citas
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

            ProductosConStockBajo = await _context.Productos
            .Where(p => p.Stock <= p.StockMinimo)
            .OrderBy(p => p.Stock)
            .Take(5)
            .Select(p => new
            {
                p.Nombre,
                p.Stock,
                p.StockMinimo
            })
            .AsNoTracking()
            .ToListAsync()
            .ContinueWith(t => t.Result
                .Select(x => (x.Nombre, x.Stock, x.StockMinimo)).ToList());

            var citas = _context.Citas
            .Include(c => c.Colaborador)
            .Where(c => c.FechaHora.Month == DateTime.Now.Month && c.FechaHora.Year == DateTime.Now.Year)
            .ToList();

            var agrupadas = citas
                .GroupBy(c => c.Colaborador.Nombre)
                .Select(g => new { Nombre = g.Key, Cantidad = g.Count() })
                .ToList();

            NombresColaboradores = agrupadas.Select(g => g.Nombre).ToList();
            CantidadCitas = agrupadas.Select(g => g.Cantidad).ToList();

            Console.WriteLine($"{CitasPorColaborador}");
        }
        
    }
}
