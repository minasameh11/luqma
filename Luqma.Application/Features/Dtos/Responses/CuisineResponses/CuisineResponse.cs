using Luqma.Application.Helpers;
using Luqma.Domain.Models;
using System.Linq.Expressions;

namespace Luqma.Application.Features.Dtos.Responses.CuisineResponses
{
    public class CuisineResponse
    {
        public Guid Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }

        public static readonly Expression<Func<Cuisine, CuisineResponse>> Projection = c => new CuisineResponse
        {
            Id = c.ID,
            NameAr = c.NameAr,
            NameEn = c.NameEn,
            ImageUrl = c.ImageUrl,
            DisplayOrder = c.DisplayOrder,
            IsActive = c.IsActive
        };

        private static readonly Func<Cuisine, CuisineResponse> Compiled = Projection.Compile();

        public static CuisineResponse From(Cuisine cuisine) => Compiled(cuisine);

        public CuisineResponse WithAbsoluteImageUrl(string baseUrl)
        {
            ImageUrl = ImageUrlResolver.ToAbsoluteUrl(baseUrl, ImageUrl);
            return this;
        }
    }
}
