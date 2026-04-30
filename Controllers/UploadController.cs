using MediaInventoryManager.Services;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace MediaInventoryManager.Controllers
{
    [ApiController]
    [Route("api/upload")]
    public class UploadController : ControllerBase
    {
        private readonly ProductCacheService _cacheService;
        private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "image/webp" };
        private const long MaxFileSize = 5 * 1024 * 1024;
        private const int MaxWidth = 800;
        public UploadController(ProductCacheService cacheService)
        {
            _cacheService = cacheService;
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No file uploaded.");

            if (image.Length > MaxFileSize)
                return BadRequest($"File size exceeds {MaxFileSize / 1024 / 1024} MB limit.");

            if (!AllowedContentTypes.Contains(image.ContentType.ToLower()))
                return BadRequest("Invalid file type. Only JPEG, PNG, and WebP images are allowed.");

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + ".webp";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = image.OpenReadStream())
            using (var img = await Image.LoadAsync(stream))
            {
                if (img.Width > MaxWidth)
                {
                    img.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(MaxWidth, 0) 
                    }));
                }

                await img.SaveAsync(filePath, new WebpEncoder
                {
                    Quality = 80
                });
            }
            _cacheService.Invalidate();
            return Ok(new { fileName = uniqueFileName });
        }
    }
}
