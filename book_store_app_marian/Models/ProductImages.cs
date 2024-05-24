using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace book_store_app_marian.Models
{
    public class ProductImages
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey("Products")]
        public Guid ProductId { get; set; }
        public virtual Products Products { get; set; }

        [Required]
        public string ProductImage { get; set; }

        [Required]
        public bool MainImage { get; set; }

        public DateTime CreatedTimestamp { get; set; }
    }
}
