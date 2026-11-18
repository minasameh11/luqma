using Luqma.Application.Helpers;
using Luqma.Domain.Models;
using System.Linq.Expressions;

namespace Luqma.Application.Features.Dtos.Responses.MenuResponses
{
    public class MenuCategoryResponse
    {
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }

        // Number of active items inside the category
        public int ItemsCount { get; set; }

        public static readonly Expression<Func<MenuCategory, MenuCategoryResponse>> Projection = c => new MenuCategoryResponse
        {
            Id = c.ID,
            StoreId = c.StoreId,
            NameAr = c.NameAr,
            NameEn = c.NameEn,
            ImageUrl = c.ImageUrl,
            DisplayOrder = c.DisplayOrder,
            IsActive = c.IsActive,
            ItemsCount = c.Items.Count(i => !i.IsArchived && i.IsActive)
        };

        private static readonly Func<MenuCategory, MenuCategoryResponse> Compiled = Projection.Compile();

        public static MenuCategoryResponse From(MenuCategory category) => Compiled(category);

        public MenuCategoryResponse WithAbsoluteImageUrl(string baseUrl)
        {
            ImageUrl = ImageUrlResolver.ToAbsoluteUrl(baseUrl, ImageUrl);
            return this;
        }
    }
}
