using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;

[ApiController]
[Route("[controller]")]
public class RetrieveController : ControllerBase
{
    private readonly string ud = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

    [HttpGet]
    public IActionResult RetrieveFile([FromQuery] string FileName, [FromQuery] string FileOwner)
    {
        if (string.IsNullOrEmpty(FileName) || string.IsNullOrEmpty(FileOwner))
        {
            return BadRequest("Invalid request. FileName and FileOwner fields are required.");
        }

        string filePath = Path.Combine(ud, FileName);
        string metadataPath = filePath + ".json";

        if (!System.IO.File.Exists(filePath) || !System.IO.File.Exists(metadataPath))
        {
            return NotFound("Invalid request. File not found.");
        }

        try
        {
            string metadataJson = System.IO.File.ReadAllText(metadataPath);
            var metadata = JsonSerializer.Deserialize<JsonDocument>(metadataJson);

            if (metadata.RootElement.TryGetProperty("Owner", out JsonElement ownerElement))
            {
                if (ownerElement.GetString() != FileOwner)
                {
                    return BadRequest("Invalid request. You are not authorized to retrieve this file.");
                }
            }
            else
            {
                return BadRequest("File metadata is invalid.");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", FileName);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal server error: " + e.Message);
        }
    }
}
