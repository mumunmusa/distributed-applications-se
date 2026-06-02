using FoodDelivery.Core.Models;
using FoodDelivery.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Data.Repositories
{
    public class OrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdAsync(int id) =>
            await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<(List<Order> Items, int Total)> GetPagedAsync(
            string? searchStatus, string? searchRestaurant,
            int page, int pageSize, string? sortBy, string sortDir,
            int userId, string role)
        {
            var q = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .AsQueryable();

            // Филтриране по роля
            if (role == "Customer")
                q = q.Where(o => o.UserId == userId);
            else if (role == "DeliveryDriver")
                q = q.Where(o => o.Status == "Confirmed" || o.Status == "Preparing" || o.Status == "OutForDelivery");
            else if (role == "RestaurantOwner")
                q = q.Where(o => o.Restaurant!.OwnerId == userId);
            // Admin вижда всички

            if (!string.IsNullOrWhiteSpace(searchStatus))
                q = q.Where(o => o.Status.Contains(searchStatus));

            if (!string.IsNullOrWhiteSpace(searchRestaurant))
                q = q.Where(o => o.Restaurant!.Name.Contains(searchRestaurant));

            q = sortBy?.ToLower() switch
            {
                "status" => sortDir == "desc" ? q.OrderByDescending(o => o.Status) : q.OrderBy(o => o.Status),
                "totalamount" => sortDir == "desc" ? q.OrderByDescending(o => o.TotalAmount) : q.OrderBy(o => o.TotalAmount),
                _ => q.OrderByDescending(o => o.CreatedAt)
            };

            var total = await q.CountAsync();
            var items = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task AddAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Order order)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}