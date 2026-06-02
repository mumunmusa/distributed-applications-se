using FoodDelivery.Core.DTOs;
using FoodDelivery.Core.Models;

namespace FoodDelivery.Core.Interfaces
{
    public interface IUserService
    {
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
        Task<User> CreateUserAsync(RegisterDto dto);
        Task<User?> GetByEmailAsync(string email);
        Task<PagedResult<UserDto>> GetAllAsync(QueryParams query);
        Task<UserDto> GetByIdAsync(int id);
        Task UpdateAsync(int id, UpdateUserDto dto);
        Task DeleteAsync(int id);
    }
}