using Microsoft.AspNetCore.Mvc;
using UniversityApplicationSystem.Services;
using UniversityApplicationSystem.Models;
using UniversityApplicationSystem.Models.ViewModels;

namespace UniversityApplicationSystem.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly ApplicationService _applicationService;
        private readonly StudentService _studentService;
        private readonly MajorService _majorService;

        public ApplicationController(
            ApplicationService applicationService,
            StudentService studentService,
            MajorService majorService)
        {
            _applicationService = applicationService;
            _studentService = studentService;
            _majorService = majorService;
        }

        public IActionResult Index()
        {
            var applications = _applicationService.GetAllApplications();
            return View(applications);
        }

        public IActionResult Create()
        {
            var viewModel = new ApplicationViewModel
            {
                Students = _studentService.GetAllStudents(),
                Majors = _majorService.GetAllMajors()
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Create(ApplicationViewModel viewModel)
        {
            if (viewModel.Application == null)
            {
                return BadRequest("Application data is required");
            }

            if (ModelState.IsValid)
            {
                _applicationService.CreateApplication(viewModel.Application);
                return RedirectToAction(nameof(Index));
            }

            viewModel.Students = _studentService.GetAllStudents();
            viewModel.Majors = _majorService.GetAllMajors();
            return View(viewModel);
        }

        public IActionResult Edit(int id)
        {
            var application = _applicationService.GetApplication(id);
            if (application == null)
            {
                return NotFound();
            }

            var viewModel = new ApplicationViewModel
            {
                Application = application,
                Students = _studentService.GetAllStudents(),
                Majors = _majorService.GetAllMajors()
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(ApplicationViewModel viewModel)
        {
            if (viewModel.Application == null)
            {
                return BadRequest("Application data is required");
            }

            if (ModelState.IsValid)
            {
                _applicationService.UpdateApplication(viewModel.Application);
                return RedirectToAction(nameof(Index));
            }

            viewModel.Students = _studentService.GetAllStudents();
            viewModel.Majors = _majorService.GetAllMajors();
            return View(viewModel);
        }

        public IActionResult Details(int id)
        {
            var application = _applicationService.GetApplicationWithDetails(id);
            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            var application = _applicationService.GetApplication(id);
            if (application == null)
            {
                return NotFound();
            }

            _applicationService.UpdateApplicationStatus(id, status, User.Identity?.Name ?? "System");
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _applicationService.DeleteApplication(id);
            return RedirectToAction(nameof(Index));
        }
    }
}