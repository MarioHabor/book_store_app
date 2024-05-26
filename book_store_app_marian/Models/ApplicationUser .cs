using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace book_store_app_marian.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Purchases> Purchases { get; set; }
        public virtual ICollection<Reviews> Reviews { get; set; }
    }
}
