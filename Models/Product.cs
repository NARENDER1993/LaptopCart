using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaptopCart.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Product name cannot be empty")]
        public string Name { get; set; }
        [StringLength(100,ErrorMessage ="Max length is 100 characters")]
        public string Description { get; set; }
        public decimal Price { get; set; }

        public string? ImagePath { get; set; }
        public DateTime Createdat { get; set; }
        [NotMapped]//it will not allow to create a column in the database for this property
        public IFormFile? ImageFile { get; set; }

        
    }
}
