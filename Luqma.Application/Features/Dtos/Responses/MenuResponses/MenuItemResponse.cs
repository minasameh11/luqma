using Luqma.Application.Helpers;
using Luqma.Domain.Models;

namespace Luqma.Application.Features.Dtos.Responses.MenuResponses
{
    public class MenuItemResponse
    {
        public Guid Id { get; set; }
        public Guid MenuCategoryId { get; set; }
        public string MenuCategoryNameAr { get; set; } = string.Empty;
        public string MenuCategoryNameEn { get; set; } = string.Empty;

        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }

        public decimal Price { get; set; }
        public decimal? CompareAtPrice { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsSoldByWeight { get; set; }
        public WeightRangeResponse? Weight { get; set; }

        public bool IsActive { get; set; }

        public List<MenuItemOptionResponse> Options { get; set; } = new();
        public List<MenuItemAddonResponse> Addons { get; set; } = new();

        public static MenuItemResponse From(MenuItem item)
        {
            return new MenuItemResponse
            {
                Id = item.ID,
                MenuCategoryId = item.MenuCategoryId,
                MenuCategoryNameAr = item.MenuCategory?.NameAr ?? string.Empty,
                MenuCategoryNameEn = item.MenuCategory?.NameEn ?? string.Empty,
                NameAr = item.NameAr,
                NameEn = item.NameEn,
                DescriptionAr = item.DescriptionAr,
                DescriptionEn = item.DescriptionEn,
                Price = item.Price,
                CompareAtPrice = item.CompareAtPrice,
                ImageUrl = item.ImageUrl,
                IsSoldByWeight = item.IsSoldByWeight,
                Weight = item.IsSoldByWeight && item.WeightMin.HasValue
                    ? new WeightRangeResponse
                    {
                        Min = item.WeightMin!.Value,
                        Max = item.WeightMax ?? item.WeightMin!.Value,
                        Step = item.WeightStep ?? 0m
                    }
                    : null,
                IsActive = item.IsActive,
                Options = item.Options
                    .Where(o => !o.IsArchived)
                    .OrderBy(o => o.CreatedDateTime)
                    .Select(o => new MenuItemOptionResponse
                    {
                        Id = o.ID,
                        NameAr = o.NameAr,
                        NameEn = o.NameEn,
                        Price = o.Price
                    })
                    .ToList(),
                Addons = item.Addons
                    .Where(a => !a.IsArchived)
                    .OrderBy(a => a.CreatedDateTime)
                    .Select(a => new MenuItemAddonResponse
                    {
                        Id = a.ID,
                        NameAr = a.NameAr,
                        NameEn = a.NameEn,
                        Price = a.Price,
                        ImageUrl = a.ImageUrl
                    })
                    .ToList()
            };
        }

        public MenuItemResponse WithAbsoluteImageUrls(string baseUrl)
        {
            ImageUrl = ImageUrlResolver.ToAbsoluteUrl(baseUrl, ImageUrl);
            foreach (var addon in Addons)
            {
                addon.ImageUrl = ImageUrlResolver.ToAbsoluteUrl(baseUrl, addon.ImageUrl);
            }

            return this;
        }
    }

    public class WeightRangeResponse
    {
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public decimal Step { get; set; }
    }

    public class MenuItemOptionResponse
    {
        public Guid Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class MenuItemAddonResponse
    {
        public Guid Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
    }
}
