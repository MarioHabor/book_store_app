using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System;

namespace book_store_app_marian.Models
{
    public class Categories
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; }

        public virtual ICollection<Products> Products { get; set; }
        public DateTime CreatedTimestamp { get; set; }
    }
}
