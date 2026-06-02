using FoodDelivery.Core.DTOs;

namespace FoodDelivery.Core.Interfaces
{
    public interface IOrderService
    {
        Task<PagedResult<OrderDto>> GetAllAsync(QueryParams query, int userId, string role);
        Task<OrderDto> GetByIdAsync(int id, int userId, string role);
        Task<OrderDto> CreateAsync(CreateOrderDto dto, int userId);
        Task UpdateStatusAsync(int id, UpdateOrderStatusDto dto, int userId, string role);
        Task DeleteAsync(int id);
    }
}