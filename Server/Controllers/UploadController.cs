using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace MyVideoResume.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public partial class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment environment;
    private readonly ILogger<UploadController> _logger;
    private readonly long _fileSizeLimit = 100 * 1024 * 1024; // 100MB size limit
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx" };

    public UploadController(IWebHostEnvironment environment, ILogger<UploadController> logger)
    {
        this.environment = environment;
        _logger = logger;
    }

    private bool ValidateFile(IFormFile file)
    {
        if (file.Length > _fileSizeLimit)
            return false;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return _allowedExtensions.Contains(extension);
    }

    private async Task<string> SaveFileAsync(IFormFile file, string customFileName = null)
    {
        var fileName = customFileName ?? $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(environment.WebRootPath, "uploads", fileName);

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return fileName;
    }

    [HttpPost("single")]
    public async Task<IActionResult> Single(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            if (!ValidateFile(file))
                return BadRequest("Invalid file type or size");

            var fileName = await SaveFileAsync(file);
            var url = Url.Content($"~/uploads/{fileName}");

            return Ok(new { Url = url });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading single file");
            return StatusCode(500, "Internal server error during file upload");
        }
    }

    [HttpPost("multiple")]
    public async Task<IActionResult> Multiple(IFormFile[] files)
    {
        try
        {
            if (files == null || !files.Any())
                return BadRequest("No files uploaded");

            var results = new List<object>();

            foreach (var file in files)
            {
                if (!ValidateFile(file))
                    continue;

                var fileName = await SaveFileAsync(file);
                var url = Url.Content($"~/uploads/{fileName}");
                results.Add(new { FileName = file.FileName, Url = url });
            }

            if (!results.Any())
                return BadRequest("No valid files uploaded");

            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading multiple files");
            return StatusCode(500, "Internal server error during files upload");
        }
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Post(IFormFile[] files, int id)
    {
        try
        {
            if (files == null || !files.Any())
                return BadRequest("No files uploaded");

            var results = new List<object>();

            foreach (var file in files)
            {
                if (!ValidateFile(file))
                    continue;

                var fileName = await SaveFileAsync(file, $"{id}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");
                var url = Url.Content($"~/uploads/{fileName}");
                results.Add(new { FileName = file.FileName, Url = url });
            }

            if (!results.Any())
                return BadRequest("No valid files uploaded");

            return Ok(new { Id = id, Files = results });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading files for ID {Id}", id);
            return StatusCode(500, "Internal server error during files upload");
        }
    }

    [HttpPost("image")]
    public async Task<IActionResult> Image(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest("No image uploaded");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!new[] { ".jpg", ".jpeg", ".png", ".gif" }.Contains(extension))
                return BadRequest("Invalid image format");

            if (file.Length > 5 * 1024 * 1024) // 5MB limit for images
                return BadRequest("Image size exceeds limit");

            var fileName = await SaveFileAsync(file);
            var url = Url.Content($"~/uploads/{fileName}");

            return Ok(new { Url = url });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image");
            return StatusCode(500, "Internal server error during image upload");
        }
    }
}
