using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using MediatR;

namespace Luqma.Application.Features.Commands.ImageCommands.UploadImageCommand
{
    public class UploadImageCommandHandler : IRequestHandler<UploadImageCommand, Response<string>>
    {
        public const long MaxFileSizeInBytes = 2 * 1024 * 1024;

        public const string MainCategoriesFolder = "uploads/main-categories";
        public const string StoresFolder = "uploads/stores";
        public const string MenuCategoriesFolder = "uploads/menu-categories";
        public const string MenuItemsFolder = "uploads/menu-items";

        private static readonly string[] AllowedExtensions = { ".png", ".jpg", ".jpeg", ".webp" };
        private static readonly string[] AllowedContentTypes = { "image/png", "image/jpeg", "image/webp" };

        private readonly IImageService _images;

        public UploadImageCommandHandler(IImageService images)
        {
            _images = images;
        }

        public async Task<Response<string>> Handle(UploadImageCommand command, CancellationToken cancellationToken)
        {
            var file = command.File;
            if (file == null || file.Length == 0)
                return Response<string>.Fail("من فضلك اختر صورة الأيقونة");

            if (file.Length > MaxFileSizeInBytes)
                return Response<string>.Fail("حجم الصورة يجب ألا يزيد عن 2 ميجابايت");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                return Response<string>.Fail("صيغة الصورة غير مدعومة، الصيغ المسموحة: png, jpg, jpeg, webp");

            if (!AllowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
                return Response<string>.Fail("نوع ملف الصورة غير صحيح");

            if (string.IsNullOrWhiteSpace(command.FolderName))
                return Response<string>.Fail("فشل في رفع الصورة");

            var url = await _images.UploadAsync(file, command.FolderName);
            if (string.IsNullOrWhiteSpace(url))
                return Response<string>.Fail("فشل في رفع الصورة");

            return Response<string>.Success(url, "تم رفع الصورة بنجاح");
        }
    }
}
