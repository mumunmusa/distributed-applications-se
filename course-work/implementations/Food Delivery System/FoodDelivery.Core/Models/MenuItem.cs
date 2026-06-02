using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Core.Models
{
    public class MenuItem
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? ImagePath { get; set; }

        public bool IsAvailable { get; set; } = true;

        public double Weight { get; set; } // в грамове

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // FK към Restaurant
        [Required]
        public int RestaurantId { get; set; }
        public Restaurant? Restaurant { get; set; }

        // Navigation
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}