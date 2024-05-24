using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace book_store_app_marian.Models
{
    public class Purchases
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey("Products")]
        public Guid ProductId { get; set; }
        public virtual Products Products { get; set; }

        [Required]
        [ForeignKey("IdentityUser")]
        public string UserId { get; set; }
        public virtual IdentityUser IdentityUser { get; set; }

        [Required]
        public double Price { get; set; }

        public DateTime CreatedTimestamp { get; set; }
    }
}
