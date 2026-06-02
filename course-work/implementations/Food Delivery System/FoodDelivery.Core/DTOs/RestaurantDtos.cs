using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Core.DTOs
{
    public class RestaurantDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Description { get; set; }
        public double Rating { get; set; }
        public string? ImagePath { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? OwnerId { get; set; }
        public string? OwnerName { get; set; }
    }

    public class CreateRestaurantDto
    {
        [Required(ErrorMessage = "Името е задължително")]
        [MaxLength(100, ErrorMessage = "Максимум 100 символа")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Адресът е задължителен")]
        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Телефонът е задължителен")]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public int? OwnerId { get; set; }
    }

    public class UpdateRestaurantDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public double? Rating { get; set; }
        public bool? IsActive { get; set; }
        public int? OwnerId { get; set; }
    }
}