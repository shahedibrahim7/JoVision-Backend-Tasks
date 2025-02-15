using Microsoft.AspNetCore.Mvc;

namespace Task44.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GreeterController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get([FromQuery] string name = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Ok("Hello anonymous");
            }
            return Ok("Hello "+name);
        }
    }
}