using Microsoft.AspNetCore.Mvc;

namespace PHE.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new[]
            {
                new
                {
                    Date = DateTime.Now,
                    TemperatureC = 30,
                    Summary = "Sunny"
                }
            });
        }
    }
}