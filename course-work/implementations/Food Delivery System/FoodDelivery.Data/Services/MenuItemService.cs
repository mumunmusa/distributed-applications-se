using FoodDelivery.Core.DTOs;
using FoodDelivery.Core.Interfaces;
using FoodDelivery.Core.Models;
using FoodDelivery.Data.Repositories;

namespace FoodDelivery.Data.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly MenuItemRepository _repo;
        private readonly RestaurantRepository _restaurantRepo;

        public MenuItemService(MenuItemRepository repo, RestaurantRepository restaurantRepo)
        {
            _repo = repo;
            _restaurantRepo = restaurantRepo;
        }

        public async Task<PagedResult<MenuItemDto>> GetAllAsync(QueryParams query, int? restaurantId)
        {
            var (items, total) = await _repo.GetPagedAsync(
                query.Search, query.Search2,
                restaurantId, query.Page, query.PageSize,
                query.SortBy, query.SortDirection);

            return new PagedResult<MenuItemDto>
            {
                Items = items.Select(m => MapToDto(m)).ToList(),
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<MenuItemDto> GetByIdAsync(int id)
        {
            var item = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Артикул с ID {id} не е намерен.");

            return MapToDto(item);
        }

        public async Task<MenuItemDto> CreateAsync(CreateMenuItemDto dto)
        {
            var restaurant = await _restaurantRepo.GetByIdAsync(dto.RestaurantId)
                ?? throw new KeyNotFoundException($"Ресторант с ID {dto.RestaurantId} не е намерен.");

            var item = new MenuItem
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Category = dto.Category,
                Weight = dto.Weight,
                RestaurantId = dto.RestaurantId
            };

            await _repo.AddAsync(item);
            return MapToDto(item);
        }

        public async Task UpdateAsync(int id, UpdateMenuItemDto dto)
        {
            var item = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Артикул с ID {id} не е намерен.");

            if (dto.Name != null) item.Name = dto.Name;
            if (dto.Description != null) item.Description = dto.Description;
            if (dto.Price.HasValue) item.Price = dto.Price.Value;
            if (dto.Category != null) item.Category = dto.Category;
            if (dto.IsAvailable.HasValue) item.IsAvailable = dto.IsAvailable.Value;
            if (dto.Weight.HasValue) item.Weight = dto.Weight.Value;

            await _repo.UpdateAsync(item);
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Артикул с ID {id} не е намерен.");

            await _repo.DeleteAsync(item);
        }

        public async Task UpdateImagePathAsync(int id, string imagePath)
        {
            var item = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Артикул с ID {id} не е намерен.");

            item.ImagePath = imagePath;
            await _repo.UpdateAsync(item);
        }

        private static MenuItemDto MapToDto(MenuItem m) => new MenuItemDto
        {
            Id = m.Id,
            Name = m.Name,
            Description = m.Description,
            Price = m.Price,
            Category = m.Category,
            ImagePath = m.ImagePath,
            IsAvailable = m.IsAvailable,
            Weight = m.Weight,
            RestaurantId = m.RestaurantId,
            RestaurantName = m.Restaurant?.Name ?? string.Empty
        };
    }
}