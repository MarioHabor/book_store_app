﻿using System.ComponentModel.DataAnnotations.Schema;
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
        //public virtual IdentityUser IdentityUser { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public String Status { get; set; }

        public virtual ICollection<Reviews> Reviews { get; set; }

        public DateTime CreatedTimestamp { get; set; }
    }
}
