using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sunrise.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "you have to provide a valid name")]
        [MinLength(3, ErrorMessage = "Name can't be less tann 3 characters.")]
        [MaxLength(30, ErrorMessage = "Name can't be more tann 30 characters.")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "you have to provide a valid name")]
        [MinLength(5, ErrorMessage = "Name can't be less tann 5 characters.")]
        [MaxLength(50, ErrorMessage = "Name can't be more tann 50 characters.")]
        public string Description { get; set; }
        [ValidateNever]
        public DateTime CreatedAt { get; set; }
        [ValidateNever]
        public DateTime LastUpdaedAt { get; set; }
        [ValidateNever]
        public List<Product> Products { get; set; }
        [ValidateNever]
        public string ImagePath { get; set; }
        [ValidateNever]
        [NotMapped]
        public IFormFile Image { get; set; }
    }
}
