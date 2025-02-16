using Microsoft.AspNetCore.Mvc;

namespace Task45.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BirthDateController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAge([FromQuery] string name = "anonymous", [FromQuery] int? year = null, 
            [FromQuery] int? month = null, [FromQuery] int? day = null)
        {
            if (year == null || month == null || day == null)
            {
                return Ok("Hello " + name + ", I can’t calculate your age without knowing your birthdate!");
            }
            try
            {
                var birthDate = new DateTime(year.Value, month.Value, day.Value);
                var today = DateTime.Today;
                var age = today.Year - birthDate.Year;
                if (birthDate > today.AddYears(-age))
                {
                    age--;
                }
                return Ok("Hello " + name + ", your age is " + age);
            }
            catch (ArgumentOutOfRangeException)
            {
                return BadRequest("Hello " + name + ", you entered an invalid birthdate.");
            }
        }
    }
}
