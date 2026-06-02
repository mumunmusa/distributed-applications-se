using FoodDelivery.Core.DTOs;

namespace FoodDelivery.Core.Interfaces
{
    public interface IMenuItemService
    {
        Task<PagedResult<MenuItemDto>> GetAllAsync(QueryParams query, int? restaurantId);
        Task<MenuItemDto> GetByIdAsync(int id);
        Task<MenuItemDto> CreateAsync(CreateMenuItemDto dto);
        Task UpdateAsync(int id, UpdateMenuItemDto dto);
        Task DeleteAsync(int id);
        Task UpdateImagePathAsync(int id, string imagePath);
    }
}