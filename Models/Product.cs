using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sunrise.Models
{
    public class Product
    {
        /// <summary>
        /// A unique identity(1,1) identifier
        /// </summary>
        public int Id { get; set; }
        [Required(ErrorMessage ="you have to provide a valid name")]
        [MinLength(3,ErrorMessage ="Name can't be less tann 3 characters.")]
        [MaxLength(30,ErrorMessage = "Name can't be more tann 30 characters.")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "you have to provide a valid name")]
        [MinLength(5, ErrorMessage = "Name can't be less tann 5 characters.")]
        [MaxLength(50, ErrorMessage = "Name can't be more tann 50 characters.")]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime ProdactionDate { get; set; }
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; }


        [ValidateNever]
        public DateTime CreatedAt { get; set; }
        [ValidateNever]
        public DateTime LastUpdaedAt { get; set; }
        [ValidateNever]
        public string ImagePath  { get; set; }
        [ValidateNever]
        [NotMapped]
        public IFormFile Image { get; set; }

    }
}
