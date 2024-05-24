using book_store_app_marian.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using book_store_app_marian.Data;
using Microsoft.EntityFrameworkCore;


namespace book_store_app_marian.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();

            var products = _context.Products
                .Include(p => p.ProductImages)
                .Take(6).ToList();

            ViewModel ViewModel = new ViewModel()
            {
                Categories = categories,
                Products = products
            };
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
