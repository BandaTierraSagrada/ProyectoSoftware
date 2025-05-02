using Microsoft.EntityFrameworkCore;
using SalonDeBelleza.src.config;
using SalonDeBelleza.src.models;
using System;

namespace SalonDeBelleza.src.services
{
    public class BotService
    {
        private readonly ApplicationDbContext _context;

        public BotService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cita>> ObtenerCitasPendientesPorTelefono(string telefono)
        {
            return await _context.Citas
                .Include(c => c.Cliente)
                .Where(c => c.Cliente.Telefono == telefono && c.Estado == "Pendiente")
                .ToListAsync();
        }
    }
}
