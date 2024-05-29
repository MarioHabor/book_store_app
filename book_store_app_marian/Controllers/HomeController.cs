using book_store_app_marian.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using book_store_app_marian.Data;
using Microsoft.EntityFrameworkCore;
using book_store_app_marian.Services;
using System.Security.Claims;


namespace book_store_app_marian.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly RecommendationService _recommendationService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, RecommendationService recommendationService)
        {
            _logger = logger;
            _context = context;
            _recommendationService = recommendationService;
        }

        [ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var categories = await _context.Categories.ToListAsync();

            var products = await _context.Products
                .Include(p => p.ProductImages)
                .Take(6).ToListAsync();

            ViewModel ViewModel = new ViewModel()
            {
                Categories = categories,
                Products = products
            };

            List<RecommendedProductViewModel> recommendations = new List<RecommendedProductViewModel>();
            if (userId != null)
            {
                recommendations = _recommendationService.GetRecommendations(userId);
            }

            ViewBag.RecommendedProducts = recommendations;

            return View(ViewModel);
        }

        public async Task<IActionResult> Privacy()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewModel ViewModel = new ViewModel()
            {
                Categories = categories
            };
            return View(ViewModel);
        }

        public async Task<IActionResult> About()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewModel ViewModel = new ViewModel()
            {
                Categories = categories
            };
            return View(ViewModel);
        }

        public async Task<IActionResult> ContactUs()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewModel ViewModel = new ViewModel()
            {
                Categories = categories
            };
            return View(ViewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
