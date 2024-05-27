using Microsoft.AspNetCore.Mvc;
using book_store_app_marian.Models;
using book_store_app_marian.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace book_store_app_marian.Controllers
{
    // only for logged users
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> PurchaseHistory()
        {
            // get logged user model
            var user = await _userManager.GetUserAsync(User);

            // Fetch user purchases and include product details
            List<Purchases> userPurchases = new List<Purchases>();
            List<Reviews> userReviewPurchases = new List<Reviews>();
            try
            {
                userPurchases = await _context.Purchases
                    .Include(p => p.Products)
                    .Include(p => p.Reviews)
                    .Where(p => p.UserId == user.Id)
                    .OrderByDescending(p => p.CreatedTimestamp)
                    .ToListAsync();

                userReviewPurchases = await _context.Reviews
                    .Include(p => p.Products)
                    .Include(p => p.Purchases)
                    .Where(p => p.UserId == user.Id)
                    .OrderByDescending(p => p.CreatedTimestamp)
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching purchases: {ex.Message}");
            }

            ViewBag.UserPurchases = userPurchases ?? new List<Purchases>();
            ViewBag.userReviewPurchases = userReviewPurchases ?? new List<Reviews>();

            ViewBag.SuccessMessage = TempData["SuccessMessage"]?.ToString();
            ViewBag.ErrorMessage = TempData["ErrorMessage"]?.ToString();

            var categories = await _context.Categories.ToListAsync();

            var viewModel = new ViewModel
            {
                Categories = categories,
            };

            return View(viewModel);
        }
    }
}
