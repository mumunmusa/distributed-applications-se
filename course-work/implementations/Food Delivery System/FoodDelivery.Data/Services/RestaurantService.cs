using FoodDelivery.Core.DTOs;
using FoodDelivery.Core.Interfaces;
using FoodDelivery.Core.Models;
using FoodDelivery.Data.Repositories;

namespace FoodDelivery.Data.Services
{

    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantRepository _repo;
        private readonly UserRepository _userRepo;

        public RestaurantService(RestaurantRepository repo, UserRepository userRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
        }

        public async Task<PagedResult<RestaurantDto>> GetAllAsync(QueryParams query)
        {
            var (items, total) = await _repo.GetPagedAsync(
                query.Search, query.Search2,
                query.Page, query.PageSize,
                query.SortBy, query.SortDirection);

            return new PagedResult<RestaurantDto>
            {
                Items = items.Select(r => MapToDto(r)).ToList(),
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<RestaurantDto> GetByIdAsync(int id)
        {
            var restaurant = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Ресторант с ID {id} не е намерен.");

            return MapToDto(restaurant);
        }

        public async Task<RestaurantDto> CreateAsync(CreateRestaurantDto dto)
        {
            var restaurant = new Restaurant
            {
                Name = dto.Name,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                Description = dto.Description,
                OwnerId = dto.OwnerId
            };

            await _repo.AddAsync(restaurant);
            return MapToDto(restaurant);
        }

        public async Task UpdateAsync(int id, UpdateRestaurantDto dto)
        {
            var restaurant = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Ресторант с ID {id} не е намерен.");

            if (dto.Name != null) restaurant.Name = dto.Name;
            if (dto.Address != null) restaurant.Address = dto.Address;
            if (dto.PhoneNumber != null) restaurant.PhoneNumber = dto.PhoneNumber;
            if (dto.Description != null) restaurant.Description = dto.Description;
            if (dto.Rating.HasValue) restaurant.Rating = dto.Rating.Value;
            if (dto.IsActive.HasValue) restaurant.IsActive = dto.IsActive.Value;
            if (dto.OwnerId.HasValue) restaurant.OwnerId = dto.OwnerId.Value;

            await _repo.UpdateAsync(restaurant);
        }

        public async Task DeleteAsync(int id)
        {
            var restaurant = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Ресторант с ID {id} не е намерен.");

            // Ако има Owner → сменяме ролята му на Customer
            if (restaurant.OwnerId.HasValue)
            {
                var owner = await _userRepo.GetByIdAsync(restaurant.OwnerId.Value);
                if (owner != null && owner.Role == "RestaurantOwner")
                {
                    owner.Role = "Customer";
                    await _userRepo.UpdateAsync(owner);
                }
            }

            await _repo.DeleteAsync(restaurant);
        }

        public async Task UpdateImagePathAsync(int id, string imagePath)
        {
            var restaurant = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Ресторант с ID {id} не е намерен.");

            restaurant.ImagePath = imagePath;
            await _repo.UpdateAsync(restaurant);
        }

        private static RestaurantDto MapToDto(Restaurant r) => new RestaurantDto
        {
            Id = r.Id,
            Name = r.Name,
            Address = r.Address,
            PhoneNumber = r.PhoneNumber,
            Description = r.Description,
            Rating = r.Rating,
            ImagePath = r.ImagePath,
            IsActive = r.IsActive,
            CreatedAt = r.CreatedAt,
            OwnerId = r.OwnerId,
            OwnerName = r.Owner?.Username
        };
    }
}