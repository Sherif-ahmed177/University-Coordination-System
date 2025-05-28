using Microsoft.AspNetCore.Mvc;
using UniversityApplicationSystem.Models;
using UniversityApplicationSystem.Models.ViewModels;
using UniversityApplicationSystem.Services;

namespace UniversityApplicationSystem.Controllers
{
    public class SchoolController : Controller
    {
        private readonly SchoolService _schoolService;
        private readonly MajorService _majorService;

        public SchoolController(SchoolService schoolService, MajorService majorService)
        {
            _schoolService = schoolService;
            _majorService = majorService;
        }

        public IActionResult Index()
        {
            var viewModel = new SchoolViewModel
            {
                Schools = _schoolService.GetAllSchools()
            };
            return View(viewModel);
        }

        public IActionResult Details(int id)
        {
            var school = _schoolService.GetSchoolWithDetails(id);
            if (school == null)
            {
                return NotFound();
            }

            var viewModel = new SchoolViewModel
            {
                School = school,
                Majors = school.Majors
            };
            return View(viewModel);
        }

        public IActionResult Create()
        {
            var viewModel = new SchoolViewModel
            {
                School = new School
                {
                    SchoolName = string.Empty,
                    Email = string.Empty,
                    Description = string.Empty,
                    Majors = new List<Major>(),
                    Students = new List<Student>()
                }
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SchoolViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.School == null)
                {
                    ModelState.AddModelError("", "School data is required");
                    return View(viewModel);
                }

                // Ensure required collections are initialized
                viewModel.School.Majors ??= new List<Major>();
                viewModel.School.Students ??= new List<Student>();

                _schoolService.AddSchool(viewModel.School);
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        public IActionResult Edit(int id)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, SchoolViewModel viewModel)
        {
            if (id != viewModel.School?.SchoolID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (viewModel.School == null)
                {
                    ModelState.AddModelError("", "School data is required");
                    return View(viewModel);
                }

                // Ensure required collections are initialized
                viewModel.School.Majors ??= new List<Major>();
                viewModel.School.Students ??= new List<Student>();

                _schoolService.UpdateSchool(viewModel.School);
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
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
    }
}