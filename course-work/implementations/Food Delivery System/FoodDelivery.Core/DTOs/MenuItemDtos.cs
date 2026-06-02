using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Core.DTOs
{
    public class MenuItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public bool IsAvailable { get; set; }
        public double Weight { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
    }

    public class CreateMenuItemDto
    {
        [Required(ErrorMessage = "Името е задължително")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Цената е задължителна")]
        [Range(0.01, 10000, ErrorMessage = "Цената трябва да е между 0.01 и 10000")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Категорията е задължителна")]
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;

        [Range(0, 10000)]
        public double Weight { get; set; }

        [Required(ErrorMessage = "Ресторантът е задължителен")]
        public int RestaurantId { get; set; }
    }

    public class UpdateMenuItemDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Range(0.01, 10000)]
        public decimal? Price { get; set; }

        [MaxLength(50)]
        public string? Category { get; set; }

        public bool? IsAvailable { get; set; }

        [Range(0, 10000)]
        public double? Weight { get; set; }
    }
}