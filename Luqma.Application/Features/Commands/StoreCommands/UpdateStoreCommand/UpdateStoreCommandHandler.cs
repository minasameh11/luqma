using Luqma.Application.Features.Commands.StoreCommands.CreateStoreCommand;
using Luqma.Application.Features.Dtos.Requests.StoreRequests;
using Luqma.Application.Features.Dtos.Responses.CuisineResponses;
using Luqma.Application.Features.Dtos.Responses.StoreResponses;
using Luqma.Application.Helpers;
using Luqma.Application.Wapper;
using Luqma.Domain.Helpers;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Commands.StoreCommands.UpdateStoreCommand
{
    public class UpdateStoreCommandHandler : IRequestHandler<UpdateStoreCommand, Response<StoreResponse>>
    {
        private readonly IGenericRepository<Store> _stores;
        private readonly IGenericRepository<MainCategory> _categories;
        private readonly IGenericRepository<Cuisine> _cuisines;
        private readonly IGenericRepository<StoreCuisine> _storeCuisines;
        private readonly IGenericRepository<StoreImage> _storeImages;
        private readonly IAccountManager _accountManager;
        private readonly ICurrentRequestService _currentRequest;

        public UpdateStoreCommandHandler(
            IGenericRepository<Store> stores,
            IGenericRepository<MainCategory> categories,
            IGenericRepository<Cuisine> cuisines,
            IGenericRepository<StoreCuisine> storeCuisines,
            IGenericRepository<StoreImage> storeImages,
            IAccountManager accountManager,
            ICurrentRequestService currentRequest)
        {
            _stores = stores;
            _categories = categories;
            _cuisines = cuisines;
            _storeCuisines = storeCuisines;
            _storeImages = storeImages;
            _accountManager = accountManager;
            _currentRequest = currentRequest;
        }

        public async Task<Response<StoreResponse>> Handle(UpdateStoreCommand command, CancellationToken cancellationToken)
        {
            var store = await _stores.GetAll()
                .FirstOrDefaultAsync(s => s.ID == command.Id && !s.IsArchived, cancellationToken);

            if (store == null)
                return Response<StoreResponse>.Fail("المحل غير موجود");

            var request = command.Request;

            // Merge the provided fields over the stored values so the result is validated as a whole
            var merged = new SaveStoreRequest
            {
                NameAr = request.NameAr ?? store.NameAr,
                NameEn = request.NameEn ?? store.NameEn,
                PhoneNumber = request.PhoneNumber ?? store.PhoneNumber,
                Address = request.Address ?? store.Address,
                IconUrl = request.IconUrl ?? store.IconUrl,
                DescriptionAr = request.DescriptionAr ?? store.DescriptionAr,
                DescriptionEn = request.DescriptionEn ?? store.DescriptionEn,
                MainCategoryId = request.MainCategoryId ?? store.MainCategoryId,
                OpenTime = request.OpenTime ?? store.OpenTime,
                CloseTime = request.CloseTime ?? store.CloseTime,
                PriceLevel = request.PriceLevel ?? store.PriceLevel,
                DeliveryTimeMinutes = request.DeliveryTimeMinutes ?? store.DeliveryTimeMinutes,
                DeliveryFee = request.DeliveryFee ?? store.DeliveryFee,
                Latitude = request.Latitude ?? store.Latitude,
                Longitude = request.Longitude ?? store.Longitude,
                IsActive = request.IsActive ?? store.IsActive,
                // Null means "keep the stored gallery", so only the request value drives the image update below
                ImageUrls = request.ImageUrls ?? new List<string>()
            };

            var error = StoreRequestValidator.Validate(merged);
            if (error != null)
                return Response<StoreResponse>.Fail(error);

            var categoryExists = await _categories.GetAll()
                .AnyAsync(c => c.ID == merged.MainCategoryId && !c.IsArchived, cancellationToken);

            if (!categoryExists)
                return Response<StoreResponse>.Fail("التصنيف غير موجود");

            var phone = merged.PhoneNumber.Trim();
            if (phone != store.PhoneNumber)
            {
                var phoneExists = await _stores.GetAll()
                    .AnyAsync(s => s.PhoneNumber == phone && s.ID != store.ID && !s.IsArchived, cancellationToken);

                if (phoneExists)
                    return Response<StoreResponse>.Fail("يوجد محل مسجل بنفس رقم التليفون");
            }

            if (request.OwnerId.HasValue)
            {
                var owner = await _accountManager.GetUserByIdAsync(request.OwnerId.Value);
                if (owner == null)
                    return Response<StoreResponse>.Fail("المستخدم غير موجود");

                store.OwnerId = request.OwnerId.Value;
            }

            List<Cuisine>? cuisinesToSet = null;
            if (request.CuisineIds != null)
            {
                var cuisineIds = request.CuisineIds.Distinct().ToList();
                cuisinesToSet = new List<Cuisine>();

                if (cuisineIds.Count > 0)
                {
                    cuisinesToSet = await _cuisines.GetAll()
                        .Where(c => cuisineIds.Contains(c.ID) && !c.IsArchived)
                        .ToListAsync(cancellationToken);

                    if (cuisinesToSet.Count != cuisineIds.Count)
                        return Response<StoreResponse>.Fail("أحد أنواع الأكل غير موجود");
                }
            }

            store.NameAr = merged.NameAr.Trim();
            store.NameEn = merged.NameEn.Trim();
            store.PhoneNumber = phone;
            store.Address = merged.Address.Trim();
            store.IconUrl = merged.IconUrl.Trim();
            store.DescriptionAr = CreateStoreCommandHandler.NormalizeDescription(merged.DescriptionAr);
            store.DescriptionEn = CreateStoreCommandHandler.NormalizeDescription(merged.DescriptionEn);
            store.MainCategoryId = merged.MainCategoryId;
            store.OpenTime = merged.OpenTime;
            store.CloseTime = merged.CloseTime;
            store.PriceLevel = merged.PriceLevel;
            store.DeliveryTimeMinutes = merged.DeliveryTimeMinutes;
            store.DeliveryFee = merged.DeliveryFee;
            store.Latitude = merged.Latitude;
            store.Longitude = merged.Longitude;
            store.IsActive = merged.IsActive;

            _stores.Update(store);

            if (cuisinesToSet != null)
            {
                var existingLinks = await _storeCuisines.GetAll()
                    .Where(sc => sc.StoreId == store.ID && !sc.IsArchived)
                    .ToListAsync(cancellationToken);

                foreach (var link in existingLinks)
                {
                    _storeCuisines.Archived(link);
                }

                foreach (var cuisine in cuisinesToSet)
                {
                    _storeCuisines.Add(new StoreCuisine { StoreId = store.ID, CuisineId = cuisine.ID });
                }
            }

            if (request.ImageUrls != null)
            {
                var imageUrls = request.ImageUrls.Select(url => url.Trim()).ToList();
                var existingImages = await _storeImages.GetAll()
                    .Where(i => i.StoreId == store.ID && !i.IsArchived)
                    .ToListAsync(cancellationToken);

                foreach (var image in existingImages)
                {
                    _storeImages.Archived(image);
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
            }

            if (!await _stores.SaveChangesAsync())
                return Response<StoreResponse>.Fail("فشل في تحديث المحل");

            // Re-read the projection so the category names match the (possibly new) category
            var response = await _stores.GetAll()
                .AsNoTracking()
                .Where(s => s.ID == store.ID)
                .Select(StoreResponse.Projection)
                .FirstAsync(cancellationToken);

            var baseUrl = _currentRequest.BaseUrl;
            response.WithAbsoluteIconUrl(baseUrl);
            response.IsOpenNow = response.IsActive
                && WorkingHours.IsOpen(response.OpenTime, response.CloseTime, TimeOnly.FromDateTime(EgyptTime.NowLocal));

            var storeCuisines = await _storeCuisines.GetAll()
                .AsNoTracking()
                .Where(sc => sc.StoreId == store.ID && !sc.IsArchived && sc.Cuisine != null)
                .Select(sc => sc.Cuisine!)
                .ToListAsync(cancellationToken);

            response.Cuisines = storeCuisines
                .OrderBy(c => c.DisplayOrder)
                .Select(c => CuisineResponse.From(c).WithAbsoluteImageUrl(baseUrl))
                .ToList();

            response.Images = await _storeImages.GetAll()
                .AsNoTracking()
                .Where(i => i.StoreId == store.ID && !i.IsArchived)
                .OrderBy(i => i.DisplayOrder)
                .Select(i => i.ImageUrl)
                .ToListAsync(cancellationToken);

            response.Images = response.Images
                .Select(url => ImageUrlResolver.ToAbsoluteUrl(baseUrl, url))
                .ToList();

            return Response<StoreResponse>.Success(response, "تم تحديث المحل بنجاح");
        }
    }
}
