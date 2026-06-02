using FoodDelivery.Core.Models;
using FoodDelivery.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Data.Repositories
{
    public class RestaurantRepository
    {
        private readonly AppDbContext _context;

        public RestaurantRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Restaurant?> GetByIdAsync(int id) =>
            await _context.Restaurants
                .Include(r => r.Owner)
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<(List<Restaurant> Items, int Total)> GetPagedAsync(
            string? searchName, string? searchAddress,
            int page, int pageSize, string? sortBy, string sortDir)
        {
            var q = _context.Restaurants
                .Include(r => r.Owner)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchName))
                q = q.Where(r => r.Name.Contains(searchName));

            if (!string.IsNullOrWhiteSpace(searchAddress))
                q = q.Where(r => r.Address.Contains(searchAddress));

            q = sortBy?.ToLower() switch
            {
                "name" => sortDir == "desc" ? q.OrderByDescending(r => r.Name) : q.OrderBy(r => r.Name),
                "rating" => sortDir == "desc" ? q.OrderByDescending(r => r.Rating) : q.OrderBy(r => r.Rating),
                "createdat" => sortDir == "desc" ? q.OrderByDescending(r => r.CreatedAt) : q.OrderBy(r => r.CreatedAt),
                _ => q.OrderBy(r => r.Name)
            };

            var total = await q.CountAsync();
            var items = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task AddAsync(Restaurant restaurant)
        {
            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Restaurant restaurant)
        {
            _context.Restaurants.Update(restaurant);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Restaurant restaurant)
        {
            _context.Restaurants.Remove(restaurant);
            await _context.SaveChangesAsync();
        }
    }
}