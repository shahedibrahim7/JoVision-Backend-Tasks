using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using Task47.Models;

[ApiController]
[Route("[controller]")]
public class TransferOwnershipController : ControllerBase
{
    private readonly string ud = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

    [HttpGet]
    public IActionResult TransferOwnership([FromQuery] string OldOwner, [FromQuery] string NewOwner)
    {
        if (string.IsNullOrEmpty(OldOwner) || string.IsNullOrEmpty(NewOwner))
        {
            return BadRequest("Both OldOwner and NewOwner are required.");
        }

        var updatedFiles = new List<object>();
        bool ownershipChanged = false;

        try
        {
            var metadataFiles = Directory.GetFiles(ud, "*.json");

            foreach (var metadataPath in metadataFiles)
            {
                string metadataJson = System.IO.File.ReadAllText(metadataPath);

                var metadata = JsonSerializer.Deserialize<Metadata>(metadataJson);

                if (metadata != null && metadata.Owner == OldOwner)
                {
                    metadata.Owner = NewOwner;
                    string updatedJson = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
                    System.IO.File.WriteAllText(metadataPath, updatedJson);

                    updatedFiles.Add(new { FileName = Path.GetFileNameWithoutExtension(metadataPath), NewOwner });
                    ownershipChanged = true;
                }
            }

            if (!ownershipChanged)
            {
                return NotFound("No files were found for the specified OldOwner.");
            }

            return Ok(new { Message = "Ownership transferred successfully.", FilesUpdated = updatedFiles });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
}
