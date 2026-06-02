using FoodDelivery.Core.DTOs;

namespace FoodDelivery.Core.Interfaces
{
    public interface IRestaurantService
    {
        Task<PagedResult<RestaurantDto>> GetAllAsync(QueryParams query);
        Task<RestaurantDto> GetByIdAsync(int id);
        Task<RestaurantDto> CreateAsync(CreateRestaurantDto dto);
        Task UpdateAsync(int id, UpdateRestaurantDto dto);
        Task DeleteAsync(int id);
        Task UpdateImagePathAsync(int id, string imagePath);
    }
}