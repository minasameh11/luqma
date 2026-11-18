using Luqma.Application.Wapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Luqma.Application.Features.Commands.ImageCommands.UploadImageCommand
{
    public class UploadImageCommand : IRequest<Response<string>>
    {
        public IFormFile? File { get; set; }

        // Server-side destination folder, e.g. UploadImageCommandHandler.StoresFolder
        public string FolderName { get; set; } = string.Empty;
    }
}
