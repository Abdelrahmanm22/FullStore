using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Store.Models
{
    public class ProductViewModel
    {
        [Required(ErrorMessage ="Product Name is required")]

        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Product Brand is required")]

        public string Brand { get; set; } = "";

        [Required(ErrorMessage = "Product Category is required")]

        public string Category { get; set; } = "";

        [Required(ErrorMessage = "Product Price is required")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Product Description is required")]
        public string Description { get; set; } = "";

        public IFormFile? Image { get; set; }
        public string? ImageFileName { get; set; }
    }
}
