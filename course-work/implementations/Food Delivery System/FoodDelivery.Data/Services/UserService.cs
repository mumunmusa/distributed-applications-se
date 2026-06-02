using FoodDelivery.Core.DTOs;
using FoodDelivery.Core.Interfaces;
using FoodDelivery.Core.Models;
using FoodDelivery.Data.Repositories;

namespace FoodDelivery.Data.Services
{
    public class UserService : IUserService
    {
        private readonly UserRepository _userRepo;

        public UserService(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<bool> EmailExistsAsync(string email) =>
            await _userRepo.EmailExistsAsync(email);

        public async Task<bool> UsernameExistsAsync(string username) =>
            await _userRepo.UsernameExistsAsync(username);

        public async Task<User> CreateUserAsync(RegisterDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "Customer",
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber
            };

            await _userRepo.AddAsync(user);
            return user;
        }

        public async Task<User?> GetByEmailAsync(string email) =>
            await _userRepo.GetByEmailAsync(email);

        public async Task<PagedResult<UserDto>> GetAllAsync(QueryParams query)
        {
            var (items, total) = await _userRepo.GetPagedAsync(
                query.Search, query.Search2,
                query.Page, query.PageSize,
                query.SortBy, query.SortDirection);

            return new PagedResult<UserDto>
            {
                Items = items.Select(u => MapToDto(u)).ToList(),
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Потребител с ID {id} не е намерен.");

            return MapToDto(user);
        }

        public async Task UpdateAsync(int id, UpdateUserDto dto)
        {
            var user = await _userRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Потребител с ID {id} не е намерен.");

            if (dto.Username != null) user.Username = dto.Username;
            if (dto.Address != null) user.Address = dto.Address;
            if (dto.PhoneNumber != null) user.PhoneNumber = dto.PhoneNumber;
            if (dto.Role != null) user.Role = dto.Role;
            if (dto.IsActive.HasValue) user.IsActive = dto.IsActive.Value;

            await _userRepo.UpdateAsync(user);
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Потребител с ID {id} не е намерен.");

            await _userRepo.DeleteAsync(user);
        }

        private static UserDto MapToDto(User u) => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            Role = u.Role,
            Address = u.Address,
            PhoneNumber = u.PhoneNumber,
            CreatedAt = u.CreatedAt,
            IsActive = u.IsActive
        };
    }
}