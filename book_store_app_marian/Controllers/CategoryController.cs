using Microsoft.AspNetCore.Mvc;
using book_store_app_marian.Models;
using book_store_app_marian.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace book_store_app_marian.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 20; // Number of products per page
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ProductCategory(Guid id, int page = 1)
        {
            var selectedCategory = _context.Categories.FirstOrDefault(c => c.Id == id);
            var productsQuery = _context.Products
                                        .Include(p => p.ProductImages)
                                        .Where(p => p.CategoryId == id);

            var totalProducts = productsQuery.Count();

            var products = productsQuery.Skip((page - 1) * PageSize)
                                        .Take(PageSize)
                                        .ToList();

            // If product page is out of range return not fund
            //if (products.IsNullOrEmpty()) { 
            //    return NotFound();
            
            //}

            var categories = await _context.Categories.ToListAsync();

            var viewModel = new ViewModel
            {
                Categories = categories,
                Products = products,
                SelectedCategoryName = selectedCategory?.CategoryName,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalProducts / (double)PageSize)
            };

            return View(viewModel);
        }
    }
}
