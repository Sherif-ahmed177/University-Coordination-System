using Microsoft.AspNetCore.Mvc;
using UniversityApplicationSystem.Services;
using UniversityApplicationSystem.Models;
using UniversityApplicationSystem.Models.ViewModels;

namespace UniversityApplicationSystem.Controllers
{
    public class MajorController : Controller
    {
        private readonly MajorService _majorService;
        private readonly SchoolService _schoolService;

        public MajorController(MajorService majorService, SchoolService schoolService)
        {
            _majorService = majorService;
            _schoolService = schoolService;
        }

        public IActionResult Index()
        {
            var majors = _majorService.GetAllMajors();
            return View(majors);
        }

        public IActionResult Create()
        {
            var viewModel = new MajorViewModel
            {
                Schools = _schoolService.GetAllSchools()
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Create(MajorViewModel viewModel)
        {
            if (viewModel.Major == null)
            {
                return BadRequest("Major data is required");
            }

            if (ModelState.IsValid)
            {
                _majorService.AddMajor(viewModel.Major);
                return RedirectToAction(nameof(Index));
            }

            viewModel.Schools = _schoolService.GetAllSchools();
            return View(viewModel);
        }

        public IActionResult Edit(int id)
        {
            var major = _majorService.GetMajor(id);
            if (major == null)
            {
                return NotFound();
            }

            var viewModel = new MajorViewModel
            {
                Major = major,
                Schools = _schoolService.GetAllSchools()
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(MajorViewModel viewModel)
        {
            if (viewModel.Major == null)
            {
                return BadRequest("Major data is required");
            }

            if (ModelState.IsValid)
            {
                _majorService.UpdateMajor(viewModel.Major);
                return RedirectToAction(nameof(Index));
            }

            viewModel.Schools = _schoolService.GetAllSchools();
            return View(viewModel);
        }

        public IActionResult Details(int id)
        {
            var major = _majorService.GetMajorWithDetails(id);
            if (major == null)
            {
                return NotFound();
            }

            return View(major);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _majorService.DeleteMajor(id);
            return RedirectToAction(nameof(Index));
        }
    }
}