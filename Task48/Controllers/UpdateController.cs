using Microsoft.AspNetCore.Mvc;
using System;
using System.IO; 
using System.Text.Json;
using System.Threading.Tasks;
using Task47.Models;

[ApiController]
[Route("[controller]")]
public class UpdateController : ControllerBase
{
    private readonly string ud = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

    [HttpPost]
    public async Task<IActionResult> UpdateFile([FromForm] Metadata request)
    {
        if (request.Image == null || request.Image.Length == 0 || request.Owner == null)
        {
            return BadRequest("Invalid request. FileName and FileOwner fields are required.");
        }

        string filePath = Path.Combine(ud, request.Image.FileName);
        string metadataPath = filePath + ".json";

        if (!System.IO.File.Exists(filePath) || !System.IO.File.Exists(metadataPath))
        {
            return BadRequest("Invalid request. File does not exist.");
        }

        try
        {
            string metadataJson = System.IO.File.ReadAllText(metadataPath);
            var metadata = JsonSerializer.Deserialize<JsonDocument>(metadataJson);

            if (metadata.RootElement.TryGetProperty("Owner", out JsonElement ownerElement))
            {
                if (ownerElement.GetString() != request.Owner)
                {
                    return Forbid("Invalid request. You are not authorized to update this file.");
                }
            }
            else
            {
                return BadRequest("File metadata is invalid.");
            }
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.Image.CopyToAsync(stream);
            }

            var updatedMetadata = new
            {
                Owner = request.Owner,
                CreationTime = System.IO.File.GetCreationTimeUtc(filePath),
                LastModified = DateTime.UtcNow
            };

            string updatedMetadataJson = JsonSerializer.Serialize(updatedMetadata);
            await System.IO.File.WriteAllTextAsync(metadataPath, updatedMetadataJson);

            return Ok("File updated successfully.");
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal server error: " + e.Message);
        }
    }
}
