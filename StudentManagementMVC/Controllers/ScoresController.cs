using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentManagementMVC.Controllers
{
    [Authorize(Roles = "Student")]
    public class ScoresController : Controller
    {
        private readonly IScoreService _scoreService;
        private readonly IStudentService _studentService;

        public ScoresController(IScoreService scoreService, IStudentService studentService)
        {
            _scoreService = scoreService;
            _studentService = studentService;
        }

        // GET: Scores/MyGrades
        public async Task<IActionResult> MyGrades()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                 // Try getting from Name if Id is not int (depends on Identity setup)
                 // For now return error if not strictly int
                 // Or query user by name to get Id
                 // Assuming Id is saved as claim
                 return RedirectToAction("Login", "Auth"); 
            }

            var currentStudent = await _studentService.GetByUserIdAsync(userId);

            if (currentStudent == null)
            {
                return View("NoProfile");
            }

            var scores = await _scoreService.GetByStudentIdAsync(currentStudent.StudentId);
            return View(scores);
        }
    }
}
