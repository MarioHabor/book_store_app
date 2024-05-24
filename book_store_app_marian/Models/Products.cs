using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace book_store_app_marian.Models
{
    public class Products
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey("Categories")]
        public Guid CategoryId { get; set; }
        public virtual Categories Categories { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        [StringLength(256, ErrorMessage = "The description cannot exceed 256 characters.")]
        public string Description { get; set; }

        public virtual ICollection<ProductImages> ProductImages { get; set; }
        public DateTime CreatedTimestamp { get; set; }


    }
}
