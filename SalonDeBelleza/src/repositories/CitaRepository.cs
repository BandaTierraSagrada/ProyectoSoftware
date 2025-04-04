using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalonDeBelleza.src.models;
using SalonDeBelleza.src.config;
using SalonDeBelleza.src.Controllers;
using SalonDeBelleza.src.services;

namespace SalonDeBelleza.src.repositories
{
    public class CitaRepository
    {
        private readonly ApplicationDbContext _context;
        
        public CitaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cita>> ObtenerCitasAsync()
        {
            return await _context.Citas.Include(c => c.Cliente).Include(c => c.Colaborador).ToListAsync();
        }

        public async Task<Cita> ObtenerCitaPorIdAsync(int id)
        {
            return await _context.Citas.FindAsync(id);
        }

        public async Task<List<Usuario>> ObtenerColaboradoresPorServicioAsync(string servicio)
        {
            return await _context.Colaboradores
                .Where(c => c.TipoServicio == servicio)
                .Select(c => c.Usuario)
                .ToListAsync();
        }

        public async Task<List<DateTime>> ObtenerHorariosDisponiblesAsync(int colaboradorId, DateTime fecha)
        {
            var colaborador = await _context.Colaboradores.FirstOrDefaultAsync(c => c.UserID == colaboradorId);
            if (colaborador == null) return new List<DateTime>();

            var citasOcupadas = await _context.Citas
                .Where(c => c.ColaboradorID == colaboradorId && c.FechaHora.Date == fecha.Date)
                .Select(c => c.FechaHora)
                .ToListAsync();

            List<DateTime> horariosDisponibles = new List<DateTime>();
            DateTime horaActual = fecha.Date + colaborador.HorarioEntrada;
            DateTime horaFin = fecha.Date + colaborador.HorarioSalida;

            while (horaActual < horaFin)
            {
                if (!citasOcupadas.Contains(horaActual))
                {
                    horariosDisponibles.Add(horaActual);
                }
                horaActual = horaActual.AddMinutes(colaborador.DuracionServicio);
            }
            return horariosDisponibles;
        }

        public async Task CrearCitaAsync(Cita cita)
        {
            _context.Citas.Add(cita);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarCitaAsync(Cita cita)
        {
            _context.Citas.Update(cita);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarCitaAsync(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita != null)
            {
                _context.Citas.Remove(cita);
                await _context.SaveChangesAsync();
            }
        }
    }
}
