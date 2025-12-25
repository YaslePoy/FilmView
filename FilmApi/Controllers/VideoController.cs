// CinemaApi/Controllers/VideoController.cs

using Microsoft.AspNetCore.Mvc;

namespace FilmApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VideoController(IWebHostEnvironment env) : Controller
{
    [HttpPost("upload")]
    public async Task<IActionResult> UploadVideo(IFormFile file)
    {
        if (file?.Length == 0) return BadRequest("Нет файла");

        var videosDir = Path.Combine(env.ContentRootPath, "videos");
        Directory.CreateDirectory(videosDir);

        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(videosDir, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var videoUrl = $"{Request.Scheme}://{Request.Host}/videos/{fileName}";
        return Ok(new { VideoUrl = videoUrl });
    }

    [HttpGet("stream/{fileName}")]
    public IActionResult StreamVideo(string fileName)
    {
        var filePath = Path.Combine(env.ContentRootPath, "videos", fileName);
        if (!System.IO.File.Exists(filePath))
            return NotFound();

        var file = System.IO.File.OpenRead(filePath);
        return File(file, "video/mp4", enableRangeProcessing: true);
    }
}