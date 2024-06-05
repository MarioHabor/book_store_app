using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using book_store_app_marian.Models;
using book_store_app_marian.Data;
using Microsoft.EntityFrameworkCore;
//using book_store_app_marian.Data.Migrations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace book_store_app_marian.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminPanelController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminPanelController> _logger;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AdminPanelController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, ApplicationDbContext context, ILogger<AdminPanelController> logger, IWebHostEnvironment hostEnvironment)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        // GET: AdminPanelController
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewModel ViewModel = new ViewModel()
            {
                Categories = categories
            };
            return View(ViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> RoleManagement()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewModel ViewModel = new ViewModel()
            {
                Categories = categories
            };
            return View(ViewModel);
        }

        // Role Management
        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && !string.IsNullOrEmpty(roleName))
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (roleExist)
                {
                    await _userManager.AddToRoleAsync(user, roleName);
                }
            }
            return RedirectToAction("Index");
        }

        // User Management
        public async Task<IActionResult> UserList()
        {
            var users = await _userManager.Users.ToListAsync();
            var categories = await _context.Categories.ToListAsync();

            ViewModel ViewModel = new ViewModel()
            {
                Categories = categories,
                Users = users,
            };
            return View(ViewModel);
        }

        // Category Management

        // GET: AdminPanel/CategoryList
        public async Task<IActionResult> CategoryList()
        {
            var categories = await _context.Categories.ToListAsync();

            ViewModel ViewModel = new ViewModel()
            {
                Categories = categories,
            };
            return View(ViewModel);

        }

        // GET: AdminPanel/CreateCategory
        public async Task<IActionResult> CreateCategory()
        {
            var categories = await _context.Categories.ToListAsync();

            ViewModel ViewModel = new ViewModel()
            {
                categoriesModel = new Models.Categories(),
                Categories = categories,
            };
            return View(ViewModel);
        }


        // POST: AdminPanel/CreateCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(ViewModel viewModel)
        {
            ModelState.Remove("Users");
            ModelState.Remove("Categories");
            ModelState.Remove("Products");
            ModelState.Remove("SelectedCategoryName");
            ModelState.Remove("categoriesModel.Products");


            if (ModelState.IsValid)
            {
                viewModel.categoriesModel.Id = Guid.NewGuid();
                _context.Categories.Add(viewModel.categoriesModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CategoryList));
            }

            // Log ModelState errors for debugging
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                _logger.LogError(error.ErrorMessage);
            }

            // Reload the categories list if model state is invalid
            viewModel.Categories = await _context.Categories.ToListAsync();

            return View(viewModel);
        }

        // GET: AdminPanel/EditCategory/5
        public async Task<IActionResult> EditCategory(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var categories = await _context.Categories.ToListAsync();

            ViewModel ViewModel = new ViewModel()
            {
                Categories = categories,
                categoriesModel = category
            };

            return View(ViewModel);
        }

        // POST: AdminPanel/EditCategory/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(Guid id, ViewModel categoryModel)
        {
            if (id != categoryModel.categoriesModel.Id)
            {
                return NotFound();
            }

            try
            {
                _context.Update(categoryModel.categoriesModel);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return RedirectToAction("EditCategory", "AdminPanel", new { id = id.ToString() });
            
        }

        // GET: AdminPanel/DeleteCategory/5
        public async Task<IActionResult> DeleteCategory(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            var categories = await _context.Categories.ToListAsync();

            ViewModel ViewModel = new ViewModel()
            {
                Categories = categories,
                categoriesModel = category
            };

            return View(ViewModel);
        }

        // POST: AdminPanel/DeleteCategory/5
        [HttpPost, ActionName("DeleteCategoryConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategoryConfirmed(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CategoryList));
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> ProductList()
        {
            var categories = await _context.Categories.ToListAsync();
            var products = await _context.Products
            .Include(p => p.ProductImages)
            .OrderByDescending(r => r.CreatedTimestamp)
            .ToListAsync();

            ViewModel ViewModel = new ViewModel()
            {
                Categories = categories,
                Products = products
            };
            return View(ViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductCreate([FromForm] string ProductName, string ProductAuthor, Guid CategoryId, double Price, string Description, int MainImageIndex, List<IFormFile> Images)
        {
           if (Images == null || Images.Count == 0)
           {
                ModelState.AddModelError("", "Please upload at least one image.");
                return View();
           }

            // Create new product
            var product = new Products
            {
                Id = Guid.NewGuid(),
                ProductName = ProductName,
                ProductAuthor = ProductAuthor,
                CategoryId = CategoryId,
                Price = Price,
                Description = Description,
                CreatedTimestamp = DateTime.Now,
                ProductImages = new List<ProductImages>()
            };

            // Save images to wwwroot/images
            for (int i = 0; i < Images.Count; i++)
            {
                var imageFile = Images[i];
                if (imageFile.Length > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                    var extension = Path.GetExtension(imageFile.FileName);
                    var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(_hostEnvironment.WebRootPath, "images", uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    var image = new ProductImages
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        ProductImage = "/images/" + uniqueFileName,
                        MainImage = i == MainImageIndex,
                        CreatedTimestamp = DateTime.Now
                    };
                    product.ProductImages.Add(image);
                }
            }

            // Save to database
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Redirect or return view as necessary
            return RedirectToAction("ProductList", "AdminPanel");
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductDelete(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("ProductList", "AdminPanel");
        }

        private bool ProductExists(Guid id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
