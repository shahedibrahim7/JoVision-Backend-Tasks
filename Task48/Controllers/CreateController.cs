using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Task47.Models;

[ApiController]
[Route("[controller]")]
public class CreateController : ControllerBase
{
    private readonly string ud = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
    [HttpPost]
    public async Task<IActionResult> UploadFile([FromForm] Metadata request)
    {
        if (request.Image == null || request.Image.Length == 0 || request.Owner == null)
        {
            return BadRequest("Invalid request. FileName and FileOwner fields are required.");
        }

        if (!Directory.Exists(ud))
        {
            Directory.CreateDirectory(ud);
        }

        string filePath = Path.Combine(ud, request.Image.FileName);
        string metadataPath = filePath + ".json";

        if (System.IO.File.Exists(filePath))
        {
            return BadRequest("Invalid request. A file with the same name already exists.");
        }

        try
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.Image.CopyToAsync(stream);
            }

            var metadata = new
            {
                Owner = request.Owner,
                CreationTime = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };
            string metadataJson = JsonSerializer.Serialize(metadata);
            await System.IO.File.WriteAllTextAsync(metadataPath, metadataJson);
            return Created("", "File uploaded successfully.");
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal server error: " + e.Message);
        }
    }
}
