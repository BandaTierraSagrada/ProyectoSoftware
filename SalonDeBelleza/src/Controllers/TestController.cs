using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalonDeBelleza.src.models; // Asegúrate de que este using apunte a tu DbContext

[ApiController]
[Route("test")]
public class TestController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TestController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("database")]
    public async Task<IActionResult> TestDatabase()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            return Ok(new { Success = canConnect });
        }
        catch (Exception ex)
        {
            // Agrega más detalles sobre el error
            return StatusCode(500, new
            {
                Error = ex.Message,
                InnerException = ex.InnerException?.Message,
                StackTrace = ex.StackTrace
            });
        }
    }
}