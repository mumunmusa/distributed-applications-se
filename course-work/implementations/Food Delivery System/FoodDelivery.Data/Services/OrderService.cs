using FoodDelivery.Core.DTOs;
using FoodDelivery.Core.Interfaces;
using FoodDelivery.Core.Models;
using FoodDelivery.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Data.Services
{
    public class OrderService : IOrderService
    {
        private readonly OrderRepository _repo;
        private readonly MenuItemRepository _menuItemRepo;
        private readonly RestaurantRepository _restaurantRepo;

        public OrderService(OrderRepository repo, MenuItemRepository menuItemRepo, RestaurantRepository restaurantRepo)
        {
            _repo = repo;
            _menuItemRepo = menuItemRepo;
            _restaurantRepo = restaurantRepo;
        }

        public async Task<PagedResult<OrderDto>> GetAllAsync(QueryParams query, int userId, string role)
        {
            var (items, total) = await _repo.GetPagedAsync(
                query.Search, query.Search2,
                query.Page, query.PageSize,
                query.SortBy, query.SortDirection,
                userId, role);

            return new PagedResult<OrderDto>
            {
                Items = items.Select(o => MapToDto(o)).ToList(),
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<OrderDto> GetByIdAsync(int id, int userId, string role)
        {
            var order = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Поръчка с ID {id} не е намерена.");

            if (role == "Customer" && order.UserId != userId)
                throw new UnauthorizedAccessException("Нямате достъп до тази поръчка.");

            return MapToDto(order);
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto dto, int userId)
        {
            var restaurant = await _restaurantRepo.GetByIdAsync(dto.RestaurantId)
                ?? throw new KeyNotFoundException($"Ресторант с ID {dto.RestaurantId} не е намерен.");

            if (!restaurant.IsActive)
                throw new InvalidOperationException("Ресторантът не е активен.");

            var order = new Order
            {
                UserId = userId,
                RestaurantId = dto.RestaurantId,
                DeliveryAddress = dto.DeliveryAddress,
                PhoneNumber = dto.PhoneNumber,
                Notes = dto.Notes,
                Status = "Pending"
            };

            foreach (var item in dto.Items)
            {
                var menuItem = await _menuItemRepo.GetByIdAsync(item.MenuItemId)
                    ?? throw new KeyNotFoundException($"Артикул с ID {item.MenuItemId} не е намерен.");

                if (menuItem.RestaurantId != dto.RestaurantId)
                    throw new ArgumentException("Всички артикули трябва да са от същия ресторант.");

                order.OrderItems.Add(new OrderItem
                {
                    MenuItemId = item.MenuItemId,
                    Quantity = item.Quantity,
                    UnitPrice = menuItem.Price
                });
            }

            order.TotalAmount = order.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity);

            await _repo.AddAsync(order);
            return MapToDto(order);
        }

        public async Task UpdateStatusAsync(int id, UpdateOrderStatusDto dto, int userId, string role)
        {
            var order = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Поръчка с ID {id} не е намерена.");

            if (order.Status == "Delivered" || order.Status == "Cancelled")
                throw new InvalidOperationException("Не може да се промени статус на завършена поръчка.");

            // Проверка на правата по роля
            if (role == "Customer")
            {
                if (order.UserId != userId)
                    throw new UnauthorizedAccessException("Нямате достъп.");
                if (dto.Status != "Cancelled" || order.Status != "Pending")
                    throw new InvalidOperationException("Клиентът може само да отмени Pending поръчка.");
            }
            else if (role == "RestaurantOwner")
            {
                var allowed = new[] { "Confirmed", "Preparing", "Cancelled" };
                if (!allowed.Contains(dto.Status))
                    throw new InvalidOperationException("Невалиден статус за RestaurantOwner.");
            }
            else if (role == "DeliveryDriver")
            {
                var allowed = new[] { "OutForDelivery", "Delivered" };
                if (!allowed.Contains(dto.Status))
                    throw new InvalidOperationException("Куриерът може само да маркира като OutForDelivery или Delivered.");
            }

            order.Status = dto.Status;
            order.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(order);
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Поръчка с ID {id} не е намерена.");

            await _repo.DeleteAsync(order);
        }

        private static OrderDto MapToDto(Order o) => new OrderDto
        {
            Id = o.Id,
            Status = o.Status,
            DeliveryAddress = o.DeliveryAddress,
            PhoneNumber = o.PhoneNumber,
            TotalAmount = o.TotalAmount,
            Notes = o.Notes,
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt,
            UserId = o.UserId,
            Username = o.User?.Username ?? string.Empty,
            RestaurantId = o.RestaurantId,
            RestaurantName = o.Restaurant?.Name ?? string.Empty,
            Items = o.OrderItems.Select(oi => new OrderItemDto
            {
                MenuItemId = oi.MenuItemId,
                MenuItemName = oi.MenuItem?.Name ?? string.Empty,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList()
        };
    }
}