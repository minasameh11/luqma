using Luqma.Application.Helpers;
using Luqma.Domain.Models;

namespace Luqma.Application.Features.Dtos.Responses.FavoriteResponses
{
    public class FavoriteResponse
    {
        public Guid Id { get; set; }
        public Guid MenuItemId { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public decimal? CompareAtPrice { get; set; }

        // False when the item is archived or no longer active
        public bool IsAvailable { get; set; }

        public DateTime? AddedAt { get; set; }

        public static FavoriteResponse From(Favorite favorite, MenuItem menuItem, string baseUrl)
        {
            return new FavoriteResponse
            {
                Id = favorite.ID,
                MenuItemId = favorite.MenuItemId,
                NameAr = menuItem.NameAr,
                NameEn = menuItem.NameEn,
                DescriptionAr = menuItem.DescriptionAr,
                DescriptionEn = menuItem.DescriptionEn,
                ImageUrl = ImageUrlResolver.ToAbsoluteUrl(baseUrl, menuItem.ImageUrl ?? string.Empty),
                Price = menuItem.Price,
                CompareAtPrice = menuItem.CompareAtPrice,
                IsAvailable = !menuItem.IsArchived && menuItem.IsActive,
                AddedAt = favorite.CreatedDateTime
            };
        }
    }
}
