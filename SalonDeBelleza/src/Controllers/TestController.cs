using Microsoft.AspNetCore.Mvc;
using SalonDeBelleza.src.services;

namespace SalonDeBelleza.src.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public TestController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpGet("test-database")]
        public IActionResult TestDatabase()
        {
            var result = _databaseService.TestConnection();
            return Ok(result);
        }
    }
}
