using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UniversityCoordinationSystem.Models;
using UniversityApplicationSystem.Services;

namespace UniversityCoordinationSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly StudentService _studentService;
        private readonly SchoolService _schoolService;
        private readonly MajorService _majorService;
        private readonly ApplicationService _applicationService;

        public HomeController(
            ILogger<HomeController> logger,
            StudentService studentService,
            SchoolService schoolService,
            MajorService majorService,
            ApplicationService applicationService)
        {
            _logger = logger;
            _studentService = studentService;
            _schoolService = schoolService;
            _majorService = majorService;
            _applicationService = applicationService;
        }

        public IActionResult Index()
        {
            ViewBag.StudentCount = _studentService.GetAllStudents().Count();
            ViewBag.SchoolCount = _schoolService.GetAllSchools().Count();
            ViewBag.MajorCount = _majorService.GetAllMajors().Count();
            ViewBag.ApplicationCount = _applicationService.GetAllApplications().Count();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
