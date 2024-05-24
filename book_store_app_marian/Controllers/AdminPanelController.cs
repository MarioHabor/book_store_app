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

        public AdminPanelController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, ApplicationDbContext context, ILogger<AdminPanelController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _logger = logger;
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

            ModelState.Remove("Users");
            ModelState.Remove("Categories");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoryModel.categoriesModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(categoryModel.categoriesModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(CategoryList));
            }

            var categories = await _context.Categories.ToListAsync();

            ViewModel ViewModel = new ViewModel()
            {
                Categories = categories,
                categoriesModel = categoryModel.categoriesModel
            };

            return View(ViewModel);
            
        }

        private bool CategoryExists(Guid id)
        {
            return _context.Categories.Any(e => e.Id == id);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategoryConfirmed(ViewModel categoryModel)
        {

            ModelState.Remove("Users");
            ModelState.Remove("Categories");
            ModelState.Remove("categoriesModel.CategoryName");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Remove(categoryModel.categoriesModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(categoryModel.categoriesModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(CategoryList));
            }

            var categories = await _context.Categories.ToListAsync();

            ViewModel ViewModel = new ViewModel()
            {
                Categories = categories,
                categoriesModel = categoryModel.categoriesModel
            };

            return View(ViewModel);
        }

        // GET: AdminPanelController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminPanelController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminPanelController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminPanelController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminPanelController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminPanelController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminPanelController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
