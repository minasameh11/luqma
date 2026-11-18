using Luqma.Application.Helpers;
using Luqma.Domain.Models;
using System.Linq.Expressions;

namespace Luqma.Application.Features.Dtos.Responses.CategoryResponses
{
    public class MainCategoryResponse
    {
        public Guid Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }

        public static readonly Expression<Func<MainCategory, MainCategoryResponse>> Projection = c => new MainCategoryResponse
        {
            Id = c.ID,
            NameAr = c.NameAr,
            NameEn = c.NameEn,
            IconUrl = c.IconUrl,
            DisplayOrder = c.DisplayOrder,
            IsActive = c.IsActive
        };

        private static readonly Func<MainCategory, MainCategoryResponse> Compiled = Projection.Compile();

        public static MainCategoryResponse From(MainCategory category) => Compiled(category);

        // Relative icon paths (e.g. /uploads/...) are turned into full URLs the client can open directly
        public MainCategoryResponse WithAbsoluteIconUrl(string baseUrl)
        {
            IconUrl = ImageUrlResolver.ToAbsoluteUrl(baseUrl, IconUrl);
            return this;
        }

        public static string ToAbsoluteUrl(string baseUrl, string url)
            => ImageUrlResolver.ToAbsoluteUrl(baseUrl, url);
    }
}
