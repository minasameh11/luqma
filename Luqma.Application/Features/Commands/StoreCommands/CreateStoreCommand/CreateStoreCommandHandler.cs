using Luqma.Application.Features.Dtos.Responses.CuisineResponses;
using Luqma.Application.Features.Dtos.Responses.StoreResponses;
using Luqma.Application.Helpers;
using Luqma.Application.Wapper;
using Luqma.Domain.Helpers;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Commands.StoreCommands.CreateStoreCommand
{
    public class CreateStoreCommandHandler : IRequestHandler<CreateStoreCommand, Response<StoreResponse>>
    {
        private readonly IGenericRepository<Store> _stores;
        private readonly IGenericRepository<MainCategory> _categories;
        private readonly IGenericRepository<Cuisine> _cuisines;
        private readonly IGenericRepository<StoreCuisine> _storeCuisines;
        private readonly IGenericRepository<StoreImage> _storeImages;
        private readonly ICurrentRequestService _currentRequest;

        public CreateStoreCommandHandler(
            IGenericRepository<Store> stores,
            IGenericRepository<MainCategory> categories,
            IGenericRepository<Cuisine> cuisines,
            IGenericRepository<StoreCuisine> storeCuisines,
            IGenericRepository<StoreImage> storeImages,
            ICurrentRequestService currentRequest)
        {
            _stores = stores;
            _categories = categories;
            _cuisines = cuisines;
            _storeCuisines = storeCuisines;
            _storeImages = storeImages;
            _currentRequest = currentRequest;
        }

        public async Task<Response<StoreResponse>> Handle(CreateStoreCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var error = StoreRequestValidator.Validate(request);
            if (error != null)
                return Response<StoreResponse>.Fail(error);

            var categoryExists = await _categories.GetAll()
                .AnyAsync(c => c.ID == request.MainCategoryId && !c.IsArchived, cancellationToken);

            if (!categoryExists)
                return Response<StoreResponse>.Fail("التصنيف غير موجود");

            var phone = request.PhoneNumber.Trim();
            var phoneExists = await _stores.GetAll()
                .AnyAsync(s => s.PhoneNumber == phone && !s.IsArchived, cancellationToken);

            if (phoneExists)
                return Response<StoreResponse>.Fail("يوجد محل مسجل بنفس رقم التليفون");

            var cuisineIds = (request.CuisineIds ?? new List<Guid>()).Distinct().ToList();
            var cuisines = new List<Cuisine>();

            if (cuisineIds.Count > 0)
            {
                cuisines = await _cuisines.GetAll()
                    .Where(c => cuisineIds.Contains(c.ID) && !c.IsArchived)
                    .ToListAsync(cancellationToken);

                if (cuisines.Count != cuisineIds.Count)
                    return Response<StoreResponse>.Fail("أحد أنواع الأكل غير موجود");
            }

            var imageUrls = (request.ImageUrls ?? new List<string>())
                .Select(url => url.Trim())
                .ToList();

            var store = new Store
            {
                NameAr = request.NameAr.Trim(),
                NameEn = request.NameEn.Trim(),
                PhoneNumber = phone,
                Address = request.Address.Trim(),
                IconUrl = request.IconUrl.Trim(),
                DescriptionAr = NormalizeDescription(request.DescriptionAr),
                DescriptionEn = NormalizeDescription(request.DescriptionEn),
                MainCategoryId = request.MainCategoryId,
                OpenTime = request.OpenTime,
                CloseTime = request.CloseTime,
                PriceLevel = request.PriceLevel,
                DeliveryTimeMinutes = request.DeliveryTimeMinutes,
                DeliveryFee = request.DeliveryFee,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                IsActive = request.IsActive
            };

            _stores.Add(store);

            foreach (var cuisine in cuisines)
            {
                _storeCuisines.Add(new StoreCuisine { StoreId = store.ID, CuisineId = cuisine.ID });
            }

            for (var index = 0; index < imageUrls.Count; index++)
            {
                _storeImages.Add(new StoreImage
                {
                    StoreId = store.ID,
                    ImageUrl = imageUrls[index],
                    DisplayOrder = index
                });
            }

            if (!await _stores.SaveChangesAsync())
                return Response<StoreResponse>.Fail("فشل في حفظ المحل");

            var baseUrl = _currentRequest.BaseUrl;
            var response = await _stores.GetAll()
                .AsNoTracking()
                .Where(s => s.ID == store.ID)
                .Select(StoreResponse.Projection)
                .FirstAsync(cancellationToken);

            response.WithAbsoluteIconUrl(baseUrl);
            response.IsOpenNow = store.IsOpenAt(TimeOnly.FromDateTime(EgyptTime.NowLocal));
            response.Cuisines = cuisines
                .OrderBy(c => c.DisplayOrder)
                .Select(c => CuisineResponse.From(c).WithAbsoluteImageUrl(baseUrl))
                .ToList();
            response.Images = imageUrls
                .Select(url => ImageUrlResolver.ToAbsoluteUrl(baseUrl, url))
                .ToList();

            return Response<StoreResponse>.Success(response, "تم إضافة المحل بنجاح");
        }

        // An empty description is stored as null so responses and filters stay consistent
        internal static string? NormalizeDescription(string? description)
        {
            var trimmed = description?.Trim();
            return string.IsNullOrEmpty(trimmed) ? null : trimmed;
        }
    }
}
