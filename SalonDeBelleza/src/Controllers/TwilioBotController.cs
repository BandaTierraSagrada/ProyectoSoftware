using Microsoft.AspNetCore.Mvc;
using SalonDeBelleza.src.services;
using Twilio.Http;
using Twilio.TwiML;
using static Google.Apis.Requests.BatchRequest;

namespace SalonDeBelleza.src.Controllers
{
    [ApiController]
    [Route("api/twilio")]
    public class TwilioBotController : ControllerBase
    {
        private readonly BotService _botService;

        public TwilioBotController(BotService botService)
        {
            _botService = botService;
        }

        [HttpPost("whatsapp")]
        public async Task<IActionResult> MensajeEntrante()
        {
            var form = await Request.ReadFormAsync();
            string from = form["From"];
            string body = form["Body"].ToString().Trim().ToLower();

            var numero = from.Replace("whatsapp:", "").Trim();
            numero = numero.Substring(4);
            var messagingResponse = new MessagingResponse();
            if (await _botService.GetUsuarioPorTelefono(numero) == 0)
            {
                messagingResponse.Message("Tu numero no esta registrado en el sitio");
                return Content(messagingResponse.ToString(), "application/xml");
            }
            /*if (body == "ver citas")
            {
                var citas = await _botService.ObtenerCitasPendientesPorTelefono(numero);
                if (!citas.Any())
                {
                    messagingResponse.Message("No tienes citas pendientes.");
                }
                else
                {
                    foreach (var cita in citas)
                    {
                        messagingResponse.Message($"Cita #{cita.CitaID} - {cita.FechaHora:dd/MM/yyyy HH:mm}\nServicio: {cita.Servicio}\nEscribe:\n*confirmar {cita.CitaID}* o *cancelar {cita.CitaID}*");
                    }
                }
            }
            else*/ 
            if (body.StartsWith("confirmar"))
            {
                if (int.TryParse(body.Split(' ')[1], out int citaId))
                {
                    if(await _botService.ConfirmarOCancelarCitaAsync(citaId, "Confirmada"))
                        messagingResponse.Message("✅ Cita confirmada.");
                    else
                        messagingResponse.Message("❌ Error al confirmar cita.");

                }
            }
            else if (body.StartsWith("cancelar"))
            {
                if (int.TryParse(body.Split(' ')[1], out int citaId))
                {
                    if(await _botService.ConfirmarOCancelarCitaAsync(citaId, "Cancelada"))
                        messagingResponse.Message("❌ Cita cancelada.");
                    else
                        messagingResponse.Message("❌ Error al cancelar cita.");
                }
            }
            else {
                var respuesta = await _botService.ProcesarMensajeAsync(numero, body);
                messagingResponse.Message(respuesta);
            }

            return Content(messagingResponse.ToString(), "application/xml");
        }

        [HttpGet("ver")]
        public async Task<IActionResult> VerCitasPorTelefono([FromQuery] string telefono)
        {
            var citas = await _botService.ObtenerCitasPendientesPorTelefono(telefono);

            return Ok(citas);
        }



    }
}
