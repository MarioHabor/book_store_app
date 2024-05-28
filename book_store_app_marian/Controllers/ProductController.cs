using book_store_app_marian.Data;
using book_store_app_marian.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace book_store_app_marian.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AdminPanelController> _logger;

        public ProductController(ApplicationDbContext context, UserManager<IdentityUser> userManager, ILogger<AdminPanelController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid id)
        {
            var product = await _context.Products
                                       .Include(p => p.ProductImages)
                                       .Include(p => p.Categories)
                                       .Include(p => p.Reviews.OrderByDescending(r => r.CreatedTimestamp))
                                       .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var categories = await _context.Categories.ToListAsync();
            var users = await _context.Users.ToListAsync();

            var viewModel = new ViewModel
            {
                categoriesModel = product.Categories,
                Categories = categories,
                Users = users,
                Products = new List<Products> { product },
                SelectedCategoryName = product.Categories?.CategoryName
            };

            ViewBag.ErrorMessage = TempData["ErrorMessage"]?.ToString();
            ViewBag.ToastSuccessMessage = TempData["ToastSuccessMessage"]?.ToString();

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

                TempData["SuccessMessage"] = "Review submitted successfully!";
                return RedirectToAction("PurchaseHistory", "User"); // Redirect to the product details page
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            // If the model state is invalid
            TempData["ErrorMessage"] = "Something went wrong.";
            return RedirectToAction("PurchaseHistory", "User");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReview([FromForm] Reviews model)
        {
            try
            {
                // get logged user model
                var user = await _userManager.GetUserAsync(User);


                var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == model.Id && r.UserId == user.Id);

                if (existingReview == null)
                {
                    return NotFound(); // Return not found if the review does not exist or does not belong to the current user
                }

                // Update the review properties
                existingReview.Review = model.Review;
                existingReview.Rating = model.Rating;
                existingReview.CreatedTimestamp = DateTime.Now; // Update timestamp if needed

                // Save the changes to the database
                _context.Reviews.Update(existingReview);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Review submitted successfully!";
                return RedirectToAction("PurchaseHistory", "User"); // Redirect to the product details page
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            // If the model state is invalid
            TempData["ErrorMessage"] = "Something went wrong.";
            return RedirectToAction("PurchaseHistory", "User");
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string searchTerm, [FromQuery] string categoryName, [FromQuery] string orderBy, [FromQuery] double? minPrice, [FromQuery] double? maxPrice, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {

            var findProduct = _context.Products
            .Where(p => p.Id.ToString() == searchTerm)
            .FirstOrDefault();

            if (findProduct != null)
            {
                TempData["ToastSuccessMessage"] = "You have been guided to the location of the book.";
                return RedirectToAction("Index", "Product", new { id = findProduct.Id.ToString() });
            }


            ViewBag.SearchTerm = searchTerm;
            ViewBag.CategoryName = categoryName;
            ViewBag.OrderBy = orderBy;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            Guid? categoryId = null;
            if (!string.IsNullOrEmpty(categoryName))
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == categoryName);
                if (category != null)
                {
                    categoryId = category.Id;
                }
            }

            var query = _context.Products.Include(p => p.Categories).Include(p => p.ProductImages).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.ProductName.Contains(searchTerm) || p.ProductAuthor.Contains(searchTerm) || p.Description.Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            switch (orderBy)
            {
                case "PriceAsc":
                    query = query.OrderBy(p => p.Price);
                    break;
                case "PriceDesc":
                    query = query.OrderByDescending(p => p.Price);
                    break;
                case "NameAsc":
                    query = query.OrderBy(p => p.ProductName);
                    break;
                case "NameDesc":
                    query = query.OrderByDescending(p => p.ProductName);
                    break;
                case "AuthorAsc":
                    query = query.OrderBy(p => p.ProductAuthor);
                    break;
                case "AuthorDesc":
                    query = query.OrderByDescending(p => p.ProductAuthor);
                    break;
            }

            var products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var totalProducts = await query.CountAsync();

            var viewModel = new ViewModel
            {
                Products = products,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize),
                Categories = await _context.Categories.ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Purchase([FromForm] Products model)
        {
            try
            {
                // get logged user model
                var user = await _userManager.GetUserAsync(User);
                // var product = await _context.Products.FindAsync(productId);
                var Purchase = new Purchases()
                {
                    Id = Guid.NewGuid(),
                    ProductId = model.Id,
                    UserId = user.Id,
                    Price = model.Price,
                    Status = "Pending",
                    CreatedTimestamp = DateTime.UtcNow

                };

                _context.Purchases.Add(Purchase);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Purchase submitted successfully!";
                return RedirectToAction("PurchaseHistory", "User");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            TempData["ErrorMessage"] = "Something went wrong.";
            return RedirectToAction("Index", "Product", new { id = model.Id.ToString() }); // Redirect to the product details page
        }
    }
}
