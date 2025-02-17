using Microsoft.AspNetCore.Mvc;
using Task46;

namespace Task46.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BirthDateController : ControllerBase
    {
        [HttpPost]
        public IActionResult GetAge([FromForm] BirthDateRequest request)
        {
            if (string.IsNullOrEmpty(request.name))
            {
                request.name = "anonymous";
            }
            if (request.year == null || request.month == null || request.day == null)
            {
                return Ok("Hello " + request.name + ", I can’t calculate your age without knowing your birthdate!");
            }

            try
            {
                var birthDate = new DateTime(request.year.Value, request.month.Value, request.day.Value);
                var today = DateTime.Today;
                var age = today.Year - birthDate.Year;

                if (birthDate > today.AddYears(-age))
                {
                    age--;
                }

                return Ok("Hello " + request.name + ", your age is " + age);
            }
            catch (ArgumentOutOfRangeException)
            {
                return BadRequest("Hello " + request.name + ", you entered an invalid birthdate.");
            }
        }
    }
}