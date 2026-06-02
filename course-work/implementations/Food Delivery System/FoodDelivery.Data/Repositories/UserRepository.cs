using FoodDelivery.Core.Models;
using FoodDelivery.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Data.Repositories
{
    public class UserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id) =>
            await _context.Users.FindAsync(id);

        public async Task<User?> GetByEmailAsync(string email) =>
            await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

        public async Task<bool> EmailExistsAsync(string email) =>
            await _context.Users.AnyAsync(u => u.Email == email);

        public async Task<bool> UsernameExistsAsync(string username) =>
            await _context.Users.AnyAsync(u => u.Username == username);

        public async Task<(List<User> Items, int Total)> GetPagedAsync(
            string? searchUsername, string? searchRole,
            int page, int pageSize, string? sortBy, string sortDir)
        {
            var q = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchUsername))
                q = q.Where(u => u.Username.Contains(searchUsername));

            if (!string.IsNullOrWhiteSpace(searchRole))
                q = q.Where(u => u.Role.Contains(searchRole));

            q = sortBy?.ToLower() switch
            {
                "username" => sortDir == "desc" ? q.OrderByDescending(u => u.Username) : q.OrderBy(u => u.Username),
                "email" => sortDir == "desc" ? q.OrderByDescending(u => u.Email) : q.OrderBy(u => u.Email),
                "createdat" => sortDir == "desc" ? q.OrderByDescending(u => u.CreatedAt) : q.OrderBy(u => u.CreatedAt),
                _ => q.OrderBy(u => u.Username)
            };

            var total = await q.CountAsync();
            var items = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}