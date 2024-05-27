using book_store_app_marian.Data;
using book_store_app_marian.Models;
using Microsoft.EntityFrameworkCore;

namespace book_store_app_marian.Services
{
    public class RecommendationService
    {
        private readonly ApplicationDbContext _context;

        public RecommendationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<RecommendedProductViewModel> GetRecommendations(string userId)
        {
           // Get the categories of the books the user has purchased
            var purchasedCategoryIds = _context.Purchases
                .Where(p => p.UserId == userId)
                .Select(p => p.Products.CategoryId)
                .Distinct()
                .ToList();

            // Recommend books from the same categories but not purchased by the user
            var recommendedProducts = _context.Products
                .Where(p => purchasedCategoryIds.Contains(p.CategoryId) &&
                            !_context.Purchases.Any(pur => pur.ProductId == p.Id && pur.UserId == userId))
                .Include(p => p.ProductImages) // Include ProductImages
                .Take(9) // limit to 9 recommendations
                .ToList();

            // Create a list of RecommendedProductViewModel
            var recommendedProductViewModels = recommendedProducts.Select(p => new RecommendedProductViewModel
            {
                Id = p.Id,
                ProductName = p.ProductName,
                ProductAuthor = p.ProductAuthor,
                Price = p.Price,
                CategoryName = p.Categories.CategoryName,
                MainImageUrl = p.ProductImages.FirstOrDefault(pi => pi.MainImage)?.ProductImage ?? "https://bootdey.com/img/Content/avatar/avatar6.png"
            }).ToList();

            return recommendedProductViewModels;
        }
    }
}
