using System.Collections.Generic;
using book_store_app_marian.Models;
using Microsoft.AspNetCore.Identity;

namespace book_store_app_marian.Models
{
    public class ViewModel
    {
        public Categories categoriesModel { get; set; }
        public List<Categories> Categories { get; set; }
        public List<IdentityUser> Users { get; set; }
        public List<Products> Products { get; set; }

        public string SelectedCategoryName { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

    }
}
