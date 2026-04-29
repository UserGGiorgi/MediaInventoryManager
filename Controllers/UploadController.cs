using Microsoft.AspNetCore.Mvc;

namespace MediaInventoryManager.Controllers
{
    [ApiController]
    [Route("api/upload")]
    public class UploadController : ControllerBase
    {
        // Allowed image MIME types
        private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "image/webp" };

        // Max file size (5 MB)
        private const long MaxFileSize = 5 * 1024 * 1024;

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No file uploaded.");

            if (image.Length > MaxFileSize)
                return BadRequest($"File size exceeds {MaxFileSize / 1024 / 1024} MB limit.");

            if (!AllowedContentTypes.Contains(image.ContentType.ToLower()))
                return BadRequest("Invalid file type. Only JPEG, PNG, and WebP images are allowed.");

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder);

            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return Ok(new { fileName = uniqueFileName });
        }
    }
}
