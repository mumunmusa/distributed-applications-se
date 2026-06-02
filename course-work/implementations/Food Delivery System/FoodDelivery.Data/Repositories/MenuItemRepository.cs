using FoodDelivery.Core.Models;
using FoodDelivery.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Data.Repositories
{
    public class MenuItemRepository
    {
        private readonly AppDbContext _context;

        public MenuItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MenuItem?> GetByIdAsync(int id) =>
            await _context.MenuItems
                .Include(m => m.Restaurant)
                .FirstOrDefaultAsync(m => m.Id == id);

        public async Task<(List<MenuItem> Items, int Total)> GetPagedAsync(
            string? searchName, string? searchCategory,
            int? restaurantId, int page, int pageSize,
            string? sortBy, string sortDir)
        {
            var q = _context.MenuItems
                .Include(m => m.Restaurant)
                .AsQueryable();

            if (restaurantId.HasValue)
                q = q.Where(m => m.RestaurantId == restaurantId.Value);

            if (!string.IsNullOrWhiteSpace(searchName))
                q = q.Where(m => m.Name.Contains(searchName));

            if (!string.IsNullOrWhiteSpace(searchCategory))
                q = q.Where(m => m.Category.Contains(searchCategory));

            q = sortBy?.ToLower() switch
            {
                "name" => sortDir == "desc" ? q.OrderByDescending(m => m.Name) : q.OrderBy(m => m.Name),
                "price" => sortDir == "desc" ? q.OrderByDescending(m => m.Price) : q.OrderBy(m => m.Price),
                "category" => sortDir == "desc" ? q.OrderByDescending(m => m.Category) : q.OrderBy(m => m.Category),
                _ => q.OrderBy(m => m.Name)
            };

            var total = await q.CountAsync();
            var items = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task AddAsync(MenuItem item)
        {
            _context.MenuItems.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MenuItem item)
        {
            _context.MenuItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(MenuItem item)
        {
            _context.MenuItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}