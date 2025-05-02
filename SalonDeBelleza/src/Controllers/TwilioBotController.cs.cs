using Microsoft.AspNetCore.Mvc;
using SalonDeBelleza.src.services;
using Twilio.TwiML;

namespace SalonDeBelleza.src.Controllers
{
    [ApiController]
    [Route("api/twilio")]
    public class TwilioBotController : ControllerBase
    {
        private readonly BotService _citaService;

        public TwilioBotController(BotService citaService)
        {
            _citaService = citaService;
        }

        [HttpPost("whatsapp")]
        public async Task<IActionResult> MensajeEntrante()
        {
            var form = await Request.ReadFormAsync();
            string from = form["From"];
            string body = form["Body"].ToString().Trim().ToLower();

            var numero = from.Replace("whatsapp:", "").Trim();

            var response = new MessagingResponse();

            if (body == "ver citas")
            {
                var citas = await _citaService.ObtenerCitasPendientesPorTelefono(numero);
                if (!citas.Any())
                {
                    response.Message("No tienes citas pendientes.");
                }
                else
                {
                    foreach (var cita in citas)
                    {
                        response.Message($"Cita #{cita.CitaID} - {cita.FechaHora:dd/MM/yyyy HH:mm}\nEscribe:\n*confirmar {cita.CitaID}* o *cancelar {cita.CitaID}*");
                    }
                }
            }
            else
            {
                response.Message("Hola 👋\nOpciones disponibles:\n- *ver citas 📆*\n- *agendar*");
            }


            return Content(response.ToString(), "application/xml");
        }
    }
}
