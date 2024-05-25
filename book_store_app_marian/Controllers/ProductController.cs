using book_store_app_marian.Data;
using book_store_app_marian.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;

namespace book_store_app_marian.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public ProductController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
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
                .Where(p => p.Id != product.Id)
                .Where(p => p.CategoryId == product.CategoryId)
                .Take(3).ToList();

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadBookInfo(Guid id)
        {
            // Retrieve the product based on the provided id
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Create the text content for the file
            var content = $"Book Title: {product.ProductName}\n\n" +
                $"Book Author:\n{product.ProductAuthor}\n\n" +
                $"Description:\n{product.Description}";

            // Convert the content to a byte array
            var byteArray = Encoding.UTF8.GetBytes(content);

            // Return the file result
            return File(byteArray, "text/plain", $"{product.ProductName}.txt");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitReview([FromForm] Reviews model)
        {
            try
            {
                // get logged user model
                var user = await _userManager.GetUserAsync(User);

                model.Id = Guid.NewGuid();
                model.UserId = user.Id;
                model.CreatedTimestamp = DateTime.UtcNow;

                _context.Reviews.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction("PurchaseHistory", "User", new { id = model.ProductId }); // Redirect to the product details page
            } catch (Exception ex)
            {

            }

            // If the model state is invalid, return the same view with the model to show validation errors
            return RedirectToAction("PurchaseHistory", "User");
        }
    }
}
