using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    public UploadController(IWebHostEnvironment env) => _env = env;

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct = default)
    {
        if (file == null || file.Length == 0) return BadRequest("No file.");
        var ext = Path.GetExtension(file.FileName);
        var name = $"{Guid.NewGuid()}{ext}";
        var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
        var dir = Path.Combine(webRoot, "uploads");
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, name);
        using var stream = System.IO.File.Create(path);
        await file.CopyToAsync(stream, ct);
        return Ok(new { url = $"/uploads/{name}" });
    }
}