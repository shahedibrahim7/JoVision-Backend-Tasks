using Microsoft.AspNetCore.Mvc;

namespace Task46.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GreeterController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromForm] string? name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Ok("Hello anonymous");
            }

            return Ok("Hello " + name);
        }
    }
}
