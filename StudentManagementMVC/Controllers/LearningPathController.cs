using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentManagementMVC.Controllers
{
    [Authorize(Roles = "Student")]
    public class LearningPathController : Controller
    {
        private readonly ILearningPathService _learningPathService;

        public LearningPathController(ILearningPathService learningPathService)
        {
            _learningPathService = learningPathService;
        }

        // GET: LearningPath
        public async Task<IActionResult> Index()
        {
             var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                var recommendation = await _learningPathService.GetLatestRecommendationAsync(userId);
                return View(recommendation);
            }
            return RedirectToAction("Login", "Auth");
        }

        // POST: LearningPath/Generate
        [HttpPost]
        public async Task<IActionResult> Generate()
        {
             var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                await _learningPathService.GenerateRecommendationAsync(userId);
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Login", "Auth");
        }
    }
}
