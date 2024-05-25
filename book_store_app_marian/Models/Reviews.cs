using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace book_store_app_marian.Models
{
    public class Reviews
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
        //public virtual IdentityUser IdentityUser { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
        [ForeignKey("Purchases")]
        public Guid PurchaseId { get; set; }
        public virtual Purchases Purchases { get; set; }

        [Required]
        [StringLength(256, ErrorMessage = "The review cannot exceed 256 characters.")]
        public string Review { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "The highest review can be 5.")]
        public byte Rating { get; set; }

        public DateTime CreatedTimestamp { get; set; }
    }
}
