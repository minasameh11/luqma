using Microsoft.AspNetCore.Http;

namespace Luqma.Domain.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadAsync(IFormFile file, string folderName);
    }
}
