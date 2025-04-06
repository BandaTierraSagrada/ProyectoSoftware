using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalonDeBelleza.src.config;
using SalonDeBelleza.src.models;
using SalonDeBelleza.src.services;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace SalonDeBelleza.src.Controllers
{
    [Route("api/notificaciones")]
    [ApiController]
    public class NotificacionesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public NotificacionesController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // ✅ Guardar o actualizar la preferencia de notificación
        [HttpPost("preferencia")]
        public async Task<IActionResult> GuardarPreferencia([FromBody] PreferenciaRequest request)
        {
            if (request == null || request.UserID <= 0)
                return BadRequest("Datos inválidos");

            // Buscar si ya tiene una preferencia registrada
            var preferencia = await _context.PreferenciasNotificaciones
                .FirstOrDefaultAsync(p => p.UserID == request.UserID);

            if (preferencia == null)
            {
                // Si no existe, la creamos
                preferencia = new PreferenciaNotificacion
                {
                    UserID = request.UserID,
                    RecibirCorreo = request.Metodo == "Correo" || request.Metodo == "Ambos",
                    RecibirWhatsApp = request.Metodo == "WhatsApp" || request.Metodo == "Ambos"
                };
                _context.PreferenciasNotificaciones.Add(preferencia);
            }
            else
            {
                // Si existe, la actualizamos
                preferencia.RecibirCorreo = request.Metodo == "Correo" || request.Metodo == "Ambos";
                preferencia.RecibirWhatsApp = request.Metodo == "WhatsApp" || request.Metodo == "Ambos";
                _context.PreferenciasNotificaciones.Update(preferencia);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Preferencia guardada correctamente." });
        }

        // ✅ Obtener preferencia de notificación de un usuario
        [HttpGet("preferencia/{userID}")]
        public async Task<IActionResult> ObtenerPreferencia(int userID)
        {
            var preferencia = await _context.PreferenciasNotificaciones
                .FirstOrDefaultAsync(p => p.UserID == userID);

            if (preferencia == null)
                return NotFound("No hay preferencia registrada.");

            string metodo = preferencia.RecibirCorreo && preferencia.RecibirWhatsApp ? "Ambos"
                         : preferencia.RecibirCorreo ? "Correo"
                         : preferencia.RecibirWhatsApp ? "WhatsApp"
                         : "Ninguno";

            return Ok(new { metodo });
        }
    }

    [Route("api/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;

        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("enviar")]
        public async Task<IActionResult> EnviarCorreo([FromBody] NotificacionRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Destinatario) ||
                string.IsNullOrEmpty(request.Asunto) || string.IsNullOrEmpty(request.CuerpoHtml))
            {
                return BadRequest(new { mensaje = "Todos los campos son obligatorios." });
            }

            bool enviado = await _emailService.EnviarCorreoAsync(request);
            if (enviado)
                return Ok(new { mensaje = "Correo enviado correctamente." });
            else
                return StatusCode(500, new { error = "No se pudo enviar el correo." });
        }
    }
    [Route("api/whatsapp")]
    [ApiController]
    public class WhatsappController : ControllerBase
    {
        private readonly WhatsAppService _whatsAppService;
        private readonly ApplicationDbContext _context;
        public WhatsappController(WhatsAppService whatsAppService, ApplicationDbContext context)
        {
            _whatsAppService = whatsAppService;
            _context = context;
        }

        [HttpPost("enviar")]
        public async Task<IActionResult> EnviarWhatsApp([FromBody] WhatsAppRequest request)
        {
            if (string.IsNullOrEmpty(request.Destinatario) || string.IsNullOrEmpty(request.Mensaje))
                return BadRequest("El destinatario y el mensaje son obligatorios.");

            await _whatsAppService.EnviarMensajeAsync(request.Destinatario, request.Mensaje);
            return Ok("Notificación enviada por WhatsApp.");
        }

    }

}
