using book_store_app_marian.Data;
using book_store_app_marian.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace book_store_app_marian.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(Guid id)
        {
            var product = await _context.Products
                                       .Include(p => p.ProductImages)
                                       .Include(p => p.Categories)
                                       .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var categories = await _context.Categories.ToListAsync();
            var users = await _context.Users.ToListAsync(); // Assuming Users is from Identity

            var viewModel = new ViewModel
            {
                categoriesModel = product.Categories,
                Categories = categories,
                Users = users,
                Products = new List<Products> { product },
                SelectedCategoryName = product.Categories?.CategoryName
            };
            // related products view bag
            ViewBag.RelatedProducts = _context.Products
                .Include(p => p.ProductImages)
                .Where(p => p.CategoryId == product.CategoryId).Take(3).ToList();

            return View(viewModel);
        }
    }
}
