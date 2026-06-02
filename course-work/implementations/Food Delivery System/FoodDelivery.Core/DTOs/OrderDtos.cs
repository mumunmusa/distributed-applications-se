using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Core.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class CreateOrderDto
    {
        [Required(ErrorMessage = "Адресът за доставка е задължителен")]
        [MaxLength(300)]
        public string DeliveryAddress { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Ресторантът е задължителен")]
        public int RestaurantId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Поръчката трябва да има поне един артикул")]
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }

    public class CreateOrderItemDto
    {
        [Required]
        public int MenuItemId { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Количеството трябва да е между 1 и 100")]
        public int Quantity { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        [Required(ErrorMessage = "Статусът е задължителен")]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;
    }
}