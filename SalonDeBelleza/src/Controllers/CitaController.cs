using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalonDeBelleza.src.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalonDeBelleza.src.config;
using SalonDeBelleza.src.services;

namespace SalonDeBelleza.src.Controllers
{
    [Route("api/citas")]
    [ApiController]
    public class CitaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly WhatsAppService _whatsAppService;
        private readonly EmailService _emailService;
        private readonly UsuarioService _usuarioService;
        public CitaController(ApplicationDbContext context, WhatsAppService whatsAppService, EmailService emailService, UsuarioService usuarioService)
        {
            _context = context;
            _whatsAppService = whatsAppService;
            _emailService = emailService;
            _usuarioService = usuarioService;
        }

        [HttpGet("colaboradores")]
        public async Task<IActionResult> GetColaboradoresPorServicio([FromQuery] string servicio)
        {
            var colaboradores = await _context.Colaboradores
                .Include(c => c.Usuario)
                .Where(c => c.TipoServicio == servicio)
                .Select(c => new { c.UserID, c.Usuario.Nombre })
                .ToListAsync();

            return Ok(colaboradores);
        }

        [HttpGet("horarios")]
        public async Task<IActionResult> GetHorariosDisponibles([FromQuery] int colaboradorId, [FromQuery] DateTime fecha)
        {
            var colaborador = await _context.Colaboradores.FindAsync(colaboradorId);
            if (colaborador == null) return NotFound("Colaborador no encontrado");

            var citas = await _context.Citas
                .Where(c => c.ColaboradorID == colaboradorId && c.FechaHora.Date == fecha.Date)
                .Select(c => c.FechaHora.TimeOfDay)
                .ToListAsync();

            var horariosDisponibles = new List<string>();
            var horaActual = colaborador.HorarioEntrada;
            var duracion = TimeSpan.FromMinutes(colaborador.DuracionServicio);

            while (horaActual + duracion <= colaborador.HorarioSalida)
            {
                bool ocupado = citas.Any(c => Math.Abs((c - horaActual).TotalMinutes) < 1);

                if (!ocupado)
                    horariosDisponibles.Add(horaActual.ToString(@"hh\:mm"));
                horaActual += duracion;
            }

            return Ok(horariosDisponibles);
        }

        [HttpPost("reservar")]
        public async Task<IActionResult> ReservarCita(
        [FromForm] int clienteID,
        [FromForm] int colaboradorID,
        [FromForm] DateTime fechaHora,
        [FromForm] string servicio,
        [FromForm] string estado,
        [FromForm] string notas)
        {
            Cita nuevaCita = new Cita { ClienteID = clienteID, ColaboradorID = colaboradorID,FechaHora=fechaHora,Servicio=servicio,Estado=estado,Notas=notas };
            Usuario user = await _usuarioService.ObtenerPorIdAsync(clienteID);
            if (!_context.Usuarios.Any(u => u.UserID == nuevaCita.ClienteID))
                return BadRequest("Cliente no válido");

            if (!_context.Colaboradores.Any(c => c.UserID == nuevaCita.ColaboradorID))
                return BadRequest("Colaborador no válido");

            _context.Citas.Add(nuevaCita);
            await _context.SaveChangesAsync();
            PreferenciaNotificacion preferencia = await _context.PreferenciasNotificaciones.FirstOrDefaultAsync(u => u.UserID == user.UserID);
            string mensaje = $"Hola {user.Nombre}, tu cita ha sido agendada para {nuevaCita.FechaHora}.";
            if (preferencia.RecibirWhatsApp)
            {
                await _whatsAppService.EnviarMensajeAsync("+521" + nuevaCita.Cliente.Telefono.ToString(), mensaje);
                Notificacion notificacionwhats = new Notificacion { UserID = user.UserID, Tipo = "WhatsApp", Destinatario = "+521" + nuevaCita.Cliente.Telefono.ToString(), Mensaje = mensaje, Enviado = true };
                _context.Notificaciones.Add(notificacionwhats);
                await _context.SaveChangesAsync();
            }
            if (preferencia.RecibirCorreo)
            {
                NotificacionRequest men_correo = new NotificacionRequest { Nombre = nuevaCita.Cliente.Nombre, Destinatario = nuevaCita.Cliente.Email, Asunto = "Agendacion cita", CuerpoHtml = mensaje };
                await _emailService.EnviarCorreoAsync(men_correo);
                Notificacion notificacioncorreo = new Notificacion { UserID = user.UserID, Tipo = "Correo", Destinatario = user.Email.ToString(), Mensaje = mensaje, Enviado = true };
                _context.Notificaciones.Add(notificacioncorreo);

                await _context.SaveChangesAsync();
            }
            return Ok("Cita reservada con éxito");
        }

        [HttpGet("agenda")]
        public async Task<IActionResult> GetAgenda([FromQuery] int? colaboradorId)
        {
            // Si el usuario es colaborador, solo ve sus citas; el admin ve todas.
            var citasQuery = _context.Citas.Include(c => c.Cliente).Include(c => c.Colaborador).AsQueryable();

            if (colaboradorId.HasValue) // Si se envía un ID, filtrar por colaborador.
            {
                citasQuery = citasQuery.Where(c => c.ColaboradorID == colaboradorId.Value);
            }

            var citas = await citasQuery
                .Select(c => new
                {
                    c.CitaID,
                    Cliente = c.Cliente.Nombre,
                    Colaborador = c.Colaborador.Nombre,
                    c.FechaHora,
                    c.Servicio,
                    c.Estado,
                    c.Notas
                })
                .ToListAsync();

            Console.WriteLine($"{citas.Count}");
            return Ok(citas);
        }

        [HttpPut("editar/{id}")]
        public async Task<IActionResult> EditarCita(int id, [FromForm] DateTime fechaHora,
        [FromForm] string estado,
        [FromForm] string notas)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita == null) return NotFound("Cita no encontrada");

            // Actualizar datos de la cita
            cita.FechaHora = fechaHora;
            cita.Estado = estado;
            cita.Notas = notas;

            await _context.SaveChangesAsync();
            Usuario user = await _usuarioService.ObtenerPorIdAsync(cita.ClienteID);
            PreferenciaNotificacion preferencia = await _context.PreferenciasNotificaciones.FirstOrDefaultAsync(u => u.UserID == user.UserID);
            string mensaje = $"Hola {user.Nombre}, tu cita ha sido modificada para {cita.FechaHora}.";
            if (preferencia.RecibirWhatsApp)
            {
                await _whatsAppService.EnviarMensajeAsync("+521" + user.Telefono.ToString(), mensaje);
            }
            if (preferencia.RecibirCorreo)
            {
                NotificacionRequest men_correo = new NotificacionRequest { Nombre=user.Nombre, Destinatario = user.Email, Asunto = "Agendacion cita", CuerpoHtml = mensaje };
                await _emailService.EnviarCorreoAsync(men_correo);
            }
            return Ok(new { message = "Cita actualizada correctamente" });
        }

        [HttpDelete("cancelar/{id}")]
        public async Task<IActionResult> CancelarCita(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita == null) return NotFound("Cita no encontrada");

            cita.Estado = "Cancelada";  // No la eliminamos, solo la marcamos como cancelada.
            await _context.SaveChangesAsync();
            Usuario user = await _usuarioService.ObtenerPorIdAsync(cita.ClienteID);
            PreferenciaNotificacion preferencia = await _context.PreferenciasNotificaciones.FirstOrDefaultAsync(u => u.UserID == user.UserID);
            string mensaje = $"Hola {user.Nombre}, tu cita ha sido cancelada del dia {cita.FechaHora}.";
            if (preferencia.RecibirWhatsApp)
            {
                await _whatsAppService.EnviarMensajeAsync("+521" + user.Telefono.ToString(), mensaje);
            }
            if (preferencia.RecibirCorreo)
            {
                NotificacionRequest men_correo = new NotificacionRequest { Nombre = user.Nombre, Destinatario = user.Email, Asunto = "Agendacion cita", CuerpoHtml = mensaje };
                await _emailService.EnviarCorreoAsync(men_correo);
            }
            return Ok(new { message = "Cita cancelada correctamente" });
        }
        [HttpPut("confirmar/{id}")]
        public async Task<IActionResult> ConfirmarCita(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita == null) return NotFound("Cita no encontrada");

            cita.Estado = "Confirmada";  // No la eliminamos, solo la marcamos como cancelada.
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cita confirmada correctamente" });
        }

        [HttpGet("detalle/{id}")]
        public async Task<IActionResult> GetCitaDetalle(int id)
        {
            var cita = await _context.Citas
                .Where(c => c.CitaID == id)
                .Select(c => new {
                    c.CitaID,
                    Cliente = c.Cliente.Nombre,
                    Colaborador = c.Colaborador.Nombre,
                    c.FechaHora,
                    c.Servicio,
                    c.Estado,
                    c.Notas
                })
                .FirstOrDefaultAsync();

            if (cita == null)
            {
                return NotFound();
            }

            return Ok(cita);
        }

        [HttpGet("cliente")]
        public async Task<IActionResult> GetCitasCliente([FromQuery] int userId)
        {
            var citas = await _context.Citas
                .Where(c => c.ClienteID == userId)
                .Select(c => new
                {
                    c.CitaID,
                    Cliente = c.Cliente.Nombre,
                    Colaborador = c.Colaborador.Nombre,
                    c.FechaHora,
                    c.Servicio,
                    c.Estado,
                    c.Notas
                }).OrderBy(c => c.FechaHora)
                .ToListAsync();

            return Ok(citas);
        }

        [HttpGet("recurrente/{userId}")]
        public async Task<IActionResult> EsClienteRecurrente(int userId)
        {
            DateTime tresMesesAtras = DateTime.UtcNow.AddMonths(-3);

            // Obtener las citas completadas del cliente en los últimos 3 meses
            var citasCompletadas = await _context.Citas
                .Where(c => c.ClienteID == userId && c.Estado == "Completada" && c.FechaHora >= tresMesesAtras)
                .CountAsync();

            bool esRecurrente = citasCompletadas >= 6;
            var response = new { UserID = userId, recurrente = esRecurrente };
            return Ok(response);
        }

        [HttpGet("buscar-cliente")]
        public async Task<IActionResult> BuscarClientePorEmail([FromQuery] string email)
        {
            var cliente = await _context.Usuarios
                .Where(u => u.Email == email && u.Rol == "Cliente")
                .Select(u => new { u.UserID, u.Nombre })
                .FirstOrDefaultAsync();

            if (cliente == null)
            {
                return NotFound(new { mensaje = "Cliente no encontrado" });
            }

            return Ok(cliente);
        }


    }
}
