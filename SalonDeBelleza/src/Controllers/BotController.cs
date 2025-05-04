using Microsoft.AspNetCore.Mvc;
using SalonDeBelleza.src.services;
using SalonDeBelleza.src.models;

namespace SalonDeBelleza.src.Controllers
{
    [ApiController]
    [Route("api/bot")]
    public class BotController : ControllerBase
    {
        private readonly BotService _citaService;

        public BotController(BotService citaService)
        {
            _citaService = citaService;
        }

        [HttpGet("pendientes")]
        public async Task<IActionResult> ObtenerCitasPendientes([FromQuery] string telefono)
        {
            var citas = await _citaService.ObtenerCitasPendientesPorTelefono(telefono);
            return Ok(citas.Select(c => new
            {
                c.CitaID,
                c.FechaHora,
                Cliente = c.Cliente.Nombre,
                Estado = c.Estado
            }));
        }

        [HttpPost("confirmar")]
        public async Task<IActionResult> ConfirmarOCancelarCita([FromBody] ConfirmacionCitaRequest request)
        {
            var exito = await _citaService.ConfirmarOCancelarCitaAsync(request.CitaID, request.Estado);
            if (!exito) return NotFound("Cita no encontrada");

            return Ok("Cita actualizada");
        }
    }
}
