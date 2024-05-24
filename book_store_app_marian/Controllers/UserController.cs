using Microsoft.AspNetCore.Mvc;
using book_store_app_marian.Models;
using book_store_app_marian.Data;
using Microsoft.EntityFrameworkCore;

namespace book_store_app_marian.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 20; // Number of products per page
        public UserController(ApplicationDbContext context) {
            _context = context;
        }
    }
}
