using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using book_store_app_marian.Services;
using System.Collections.Generic;
using book_store_app_marian.Models;

namespace book_store_app_marian.Controllers
{
    public class RecommendationsController : Controller
    {
        private readonly RecommendationService _recommendationService;

        public RecommendationsController(RecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var recommendations = _recommendationService.GetRecommendations(userId);
            return View(recommendations);
        }
    }
}
