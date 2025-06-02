using Microsoft.AspNetCore.Mvc;
using UniversityApplicationSystem.Models;
using UniversityApplicationSystem.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Collections.Generic;
using System.Linq;

namespace UniversityApplicationSystem.Controllers
{
    public class MajorController : Controller
    {
        private readonly MajorService _majorService;
        private readonly SchoolService _schoolService;
        private readonly ILogger<MajorController> _logger;

        public MajorController(MajorService majorService, SchoolService schoolService, ILogger<MajorController> logger)
        {
            _majorService = majorService;
            _schoolService = schoolService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var majors = _majorService.GetAllMajors();
            return View(majors);
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

        public IActionResult Create()
        {
            ViewBag.Schools = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_schoolService.GetAllSchools(), "SchoolID", "SchoolName");
            return View(new Major
            {
                MajorName = string.Empty,
                Description = string.Empty,
                School = null,
                Applications = new List<Application>()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Major major)
        {
            var school = _schoolService.GetSchool(major.SchoolID);
            if (school == null)
            {
                ModelState.AddModelError("SchoolID", "Selected school does not exist");
            }
            else if (school.TotalScales.HasValue && major.Capacity.HasValue)
            {
                // Get all existing majors for this school
                var allMajors = _majorService.GetAllMajors().Where(m => m.SchoolID == major.SchoolID && m.Capacity.HasValue);
                int currentTotal = allMajors.Sum(m => m.Capacity.Value);
                int newTotal = currentTotal + major.Capacity.Value;
                if (newTotal > school.TotalScales.Value)
                {
                    ModelState.AddModelError("Capacity", $"The total capacity of all majors for this school (including this one: {newTotal}) cannot exceed the school's total scales ({school.TotalScales.Value}).");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _majorService.AddMajor(major);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating major: " + ex.Message);
                }
            }

            // Get the school for the dropdown
            ViewBag.Schools = new SelectList(_schoolService.GetAllSchools(), "SchoolID", "SchoolName", major.SchoolID);
            return View(major);
        }

        public IActionResult Edit(int id)
        {
            var major = _majorService.GetMajorWithDetails(id);
            if (major == null)
            {
                return NotFound();
            }

            // Get the school for the dropdown
            var school = _schoolService.GetSchool(major.SchoolID);
            ViewBag.Schools = new SelectList(_schoolService.GetAllSchools(), "SchoolID", "SchoolName", major.SchoolID);
            return View(major);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Major major)
        {
            if (id != major.MajorID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var school = _schoolService.GetSchool(major.SchoolID);
                if (school == null)
                {
                    ModelState.AddModelError("SchoolID", "Selected school does not exist");
                    ViewBag.Schools = new SelectList(_schoolService.GetAllSchools(), "SchoolID", "SchoolName");
                    return View(major);
                }
                major.School = school;
                major.Applications ??= new List<Application>();

                try
                {
                    _majorService.UpdateMajor(major);
                    TempData["SuccessMessage"] = "Major updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating major");
                    ModelState.AddModelError("", "An error occurred while updating the major. Please try again.");
                }
            }

            ViewBag.Schools = new SelectList(_schoolService.GetAllSchools(), "SchoolID", "SchoolName");
            return View(major);
        }

        public IActionResult Delete(int id)
        {
            var major = _majorService.GetMajor(id);
            if (major == null)
            {
                return NotFound();
            }
            return View(major);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _majorService.DeleteMajor(id);
            TempData["SuccessMessage"] = "Major deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}