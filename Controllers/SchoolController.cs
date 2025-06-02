using Microsoft.AspNetCore.Mvc;
using UniversityApplicationSystem.Models;
using UniversityApplicationSystem.Models.ViewModels;
using UniversityApplicationSystem.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace UniversityApplicationSystem.Controllers
{
    public class SchoolController : Controller
    {
        private readonly SchoolService _schoolService;
        private readonly MajorService _majorService;
        private readonly ILogger<SchoolController> _logger;

        public SchoolController(SchoolService schoolService, MajorService majorService, ILogger<SchoolController> logger)
        {
            _schoolService = schoolService;
            _majorService = majorService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var schools = _schoolService.GetAllSchools();
            return View(schools);
        }

        public IActionResult Details(int id)
        {
            var school = _schoolService.GetSchoolWithDetails(id);
            if (school == null)
            {
                return NotFound();
            }
            return View(school);
        }

        public IActionResult Create()
        {
            var school = new School
            {
                SchoolName = string.Empty,
                Email = string.Empty,
                Description = string.Empty,
                TotalScales = null,
                MinRequiredGrade = null,
                EstablishedYear = null,
                Majors = new List<Major>(),
                Students = new List<Student>()
            };
            return View(school);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(School school)
        {
            _logger.LogInformation("Create action started with model state: {IsValid}", ModelState.IsValid);
            
            if (school == null)
            {
                _logger.LogWarning("School data is null in Create action");
                ModelState.AddModelError("", "School data is required");
                return View(school);
            }

            _logger.LogInformation("School data received: Name={Name}, Email={Email}, Description={Description}, TotalScales={TotalScales}, MinRequiredGrade={MinRequiredGrade}, EstablishedYear={EstablishedYear}",
                school.SchoolName,
                school.Email,
                school.Description,
                school.TotalScales,
                school.MinRequiredGrade,
                school.EstablishedYear);

            if (ModelState.IsValid)
            {
                try
                {
                    // Ensure required collections are initialized
                    school.Majors ??= new List<Major>();
                    school.Students ??= new List<Student>();

                    _logger.LogInformation("Attempting to add school with data: {SchoolData}", 
                        JsonSerializer.Serialize(school));

                    var result = _schoolService.AddSchool(school);
                    _logger.LogInformation("School added successfully with ID: {Result}", result);
                    TempData["SuccessMessage"] = "School created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while creating school: {ErrorMessage}", ex.Message);
                    ModelState.AddModelError("", "An error occurred while creating the school: " + ex.Message);
                }
            }
            else
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                _logger.LogWarning("Model state is invalid. Errors: {Errors}", string.Join(", ", errors));
            }

            return View(school);
        }

        public IActionResult Edit(int id)
        {
            var school = _schoolService.GetSchool(id);
            if (school == null)
            {
                return NotFound();
            }
            return View(school);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, School school)
        {
            if (id != school.SchoolID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (school == null)
                {
                    ModelState.AddModelError("", "School data is required");
                    return View(school);
                }

                // Ensure required collections are initialized
                school.Majors ??= new List<Major>();
                school.Students ??= new List<Student>();

                _schoolService.UpdateSchool(school);
                return RedirectToAction(nameof(Index));
            }
            return View(school);
        }

        public IActionResult Delete(int id)
        {
            var school = _schoolService.GetSchool(id);
            if (school == null)
            {
                return NotFound();
            }

            var viewModel = new SchoolViewModel
            {
                School = school
            };
            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _schoolService.DeleteSchool(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: School/AddMajor/5
        public IActionResult AddMajor(int id)
        {
            var school = _schoolService.GetSchool(id);
            if (school == null)
            {
                return NotFound();
            }

            var viewModel = new MajorViewModel
            {
                SchoolID = id,
                SchoolName = school.SchoolName
            };
            return View(viewModel);
        }

        // POST: School/AddMajor/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddMajor(int id, MajorViewModel viewModel)
        {
            if (id != viewModel.SchoolID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var school = _schoolService.GetSchool(id);
                if (school == null)
                {
                    return NotFound();
                }

                var major = new Major
                {
                    MajorName = viewModel.Name,
                    Description = viewModel.Description,
                    SchoolID = viewModel.SchoolID,
                    Capacity = viewModel.Capacity,
                    DurationYears = viewModel.DurationYears,
                    School = school,
                    Applications = new List<Application>()
                };

                _majorService.AddMajor(major);
                TempData["SuccessMessage"] = $"Major '{viewModel.Name}' has been added successfully to {school.SchoolName}.";
                return RedirectToAction(nameof(Details), new { id = viewModel.SchoolID });
            }
            return View(viewModel);
        }
    }
}