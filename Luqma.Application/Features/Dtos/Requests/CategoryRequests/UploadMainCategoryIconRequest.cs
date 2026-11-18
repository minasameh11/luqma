using Microsoft.AspNetCore.Http;

namespace Luqma.Application.Features.Dtos.Requests.CategoryRequests
{
    public class UploadMainCategoryIconRequest
    {
        public IFormFile? File { get; set; }
    }
}
