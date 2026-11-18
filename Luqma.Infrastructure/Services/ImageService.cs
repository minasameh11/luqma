using Luqma.Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Luqma.Infrastructure.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> UploadAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            var wwwrootPath = _webHostEnvironment.WebRootPath;
            
            // Fallback to Directory.GetCurrentDirectory() if WebRootPath is null (e.g. in some unit tests or scenarios where wwwroot is missing)
            if (string.IsNullOrEmpty(wwwrootPath))
            {
                wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            var folderPath = Path.Combine(wwwrootPath, folderName);
            
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{folderName}/{fileName}";
        }
    }
}
