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
        [StringLength(100, ErrorMessage = "The author name cannot exceed 100 characters.")]
        public string ProductAuthor { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The product name name cannot exceed 100 characters.")]
        public string ProductName { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string Description { get; set; }

        public virtual ICollection<ProductImages> ProductImages { get; set; }

        public virtual ICollection<Reviews> Reviews { get; set; }
        public DateTime CreatedTimestamp { get; set; }


    }
}
