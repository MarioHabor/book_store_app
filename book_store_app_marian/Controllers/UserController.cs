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
        private Task<IdentityUser> GetCurrentUserAsync() => _userManager.GetUserAsync(User); // get logged user model
        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager) {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> PrchaseHistory()
        {
            var user = await GetCurrentUserAsync();

            var userPurchases = _context.Purchases
                .Where(p => p.UserId == user.Id)
                .ToListAsync();

            ViewBag.Purchases = userPurchases;

            var categories = await _context.Categories.ToListAsync();

            var viewModel = new ViewModel
            {
                Categories = categories,
            };

            return View(viewModel);
        }
    }
}
