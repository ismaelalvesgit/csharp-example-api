using Example.Application.Dto;
using Example.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Example.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ProducesErrorResponseType(typeof(ExceptionDto))]
    public class HealthcheckController : Controller
    {
        private readonly AppDbContext _context;

        public HealthcheckController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Health()
        {
            await _context.Category.AnyAsync();

            return Ok();
        }
    }
}