using Microsoft.AspNetCore.Mvc;
using UniversityApplicationSystem.Services;
using UniversityApplicationSystem.Models;
using UniversityApplicationSystem.Models.ViewModels;
using System.Data;
using Microsoft.Extensions.Logging;

namespace UniversityApplicationSystem.Controllers
{
    public class StudentController : Controller
    {
        private readonly StudentService _studentService;
        private readonly SchoolService _schoolService;
        private readonly ILogger<StudentController> _logger;

        public StudentController(StudentService studentService, SchoolService schoolService, ILogger<StudentController> logger)
        {
            _studentService = studentService;
            _schoolService = schoolService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var students = _studentService.GetAllStudents();
            return View(students);
        }

        public IActionResult Create()
        {
            var viewModel = new StudentViewModel
            {
                Student = new Student
                {
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    Email = string.Empty,
                    Gender = string.Empty,
                    NationalIDNumber = string.Empty,
                    DateOfBirth = DateTime.Now.AddYears(-18) // Default to 18 years ago
                },
                Schools = _schoolService.GetAllSchools()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(StudentViewModel viewModel)
        {
            _logger.LogInformation("Create action started with model state: {IsValid}", ModelState.IsValid);
            
            if (viewModel.Student == null)
            {
                _logger.LogWarning("Student data is null in Create action");
                ModelState.AddModelError("", "Student data is required");
                viewModel.Schools = _schoolService.GetAllSchools();
                return View(viewModel);
            }

            _logger.LogInformation("Student data received: FirstName={FirstName}, LastName={LastName}, Email={Email}, SchoolID={SchoolID}",
                viewModel.Student.FirstName,
                viewModel.Student.LastName,
                viewModel.Student.Email,
                viewModel.Student.SchoolID);

            if (ModelState.IsValid)
            {
                try
                {
                    // Create a new student object with only the necessary properties
                    var student = new Student
                    {
                        FirstName = viewModel.Student.FirstName,
                        LastName = viewModel.Student.LastName,
                        Email = viewModel.Student.Email,
                        DateOfBirth = viewModel.Student.DateOfBirth,
                        Gender = viewModel.Student.Gender,
                        NationalIDNumber = viewModel.Student.NationalIDNumber,
                        SchoolID = viewModel.Student.SchoolID
                    };

                    var result = _studentService.AddStudent(student);
                    _logger.LogInformation("Student added successfully with result: {Result}", result);
                    TempData["SuccessMessage"] = "Student created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while creating student");
                    ModelState.AddModelError("", "An error occurred while creating the student: " + ex.Message);
                }
            }
            else
            {
                _logger.LogWarning("Model state is invalid. Errors: {Errors}", 
                    string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)));
            }

            viewModel.Schools = _schoolService.GetAllSchools();
            return View(viewModel);
        }

        public IActionResult Edit(int id)
        {
            var student = _studentService.GetStudent(id);
            if (student == null)
            {
                return NotFound();
            }

            var viewModel = new StudentViewModel
            {
                Student = student,
                Schools = _schoolService.GetAllSchools()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(StudentViewModel viewModel)
        {
            if (viewModel.Student == null)
            {
                return BadRequest("Student data is required");
            }

            if (ModelState.IsValid)
            {
                _studentService.UpdateStudent(viewModel.Student);
                return RedirectToAction(nameof(Index));
            }

            viewModel.Schools = _schoolService.GetAllSchools();
            return View(viewModel);
        }

        public IActionResult Details(int id)
        {
            var student = _studentService.GetStudentWithDetails(id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _studentService.DeleteStudent(id);
            return RedirectToAction(nameof(Index));
        }
    }
}