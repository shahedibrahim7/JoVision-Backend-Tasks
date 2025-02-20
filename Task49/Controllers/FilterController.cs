using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Task47.Models;

[ApiController]
[Route("[controller]")]
public class FilterController : ControllerBase
{
    private readonly string ud = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

    [HttpPost]
    public IActionResult FilterFiles([FromForm] FilterRequest request)
    {
        if (request.FilterType == null || (string.IsNullOrEmpty(request.Owner) && request.FilterType != FilterType.ByOwner))
        {
            return BadRequest("Invalid request. All fields are required.");
        }

        try
        {
            var filePaths = Directory.GetFiles(ud);
            var filteredFiles = new List<object>();

            foreach (var filePath in filePaths)
            {
                var metadataPath = filePath + ".json";
                if (!System.IO.File.Exists(metadataPath))
                {
                    continue;
                }

                var metadataJson = System.IO.File.ReadAllText(metadataPath);
                var metadata = JsonSerializer.Deserialize<JsonDocument>(metadataJson);

                var fileName = Path.GetFileName(filePath);
                var owner = metadata.RootElement.GetProperty("Owner").GetString();
                var creationTime = System.IO.File.GetCreationTimeUtc(filePath);
                var lastModifiedTime = System.IO.File.GetLastWriteTimeUtc(filePath);

                if (request.FilterType == FilterType.ByModificationDate && lastModifiedTime < request.ModificationDate)
                {
                    filteredFiles.Add(new { FileName = fileName, Owner = owner });
                }
                else if (request.FilterType == FilterType.ByCreationDateDescending && creationTime > request.CreationDate)
                {
                    filteredFiles.Add(new { FileName = fileName, Owner = owner });
                }
                else if (request.FilterType == FilterType.ByCreationDateAscending && creationTime > request.CreationDate)
                {
                    filteredFiles.Add(new { FileName = fileName, Owner = owner });
                }
                else if (request.FilterType == FilterType.ByOwner && owner == request.Owner)
                {
                    filteredFiles.Add(new { FileName = fileName, Owner = owner });
                }
            }

            if (filteredFiles.Count == 0)
            {
                return NotFound("No files found matching the filter.");
            }

            return Ok(filteredFiles);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal server error: " + e.Message);
        }
    }
}
