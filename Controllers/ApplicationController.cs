using Microsoft.AspNetCore.Mvc;
using UniversityApplicationSystem.Services;
using UniversityApplicationSystem.Models;
using UniversityApplicationSystem.Models.ViewModels;
using Microsoft.Extensions.Logging;

namespace UniversityApplicationSystem.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly ApplicationService _applicationService;
        private readonly StudentService _studentService;
        private readonly MajorService _majorService;
        private readonly PaymentService _paymentService;
        private readonly ILogger<ApplicationController> _logger;

        public ApplicationController(
            ApplicationService applicationService,
            StudentService studentService,
            MajorService majorService,
            PaymentService paymentService,
            ILogger<ApplicationController> logger)
        {
            _applicationService = applicationService;
            _studentService = studentService;
            _majorService = majorService;
            _paymentService = paymentService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var applications = _applicationService.GetAllApplications();
            var viewModels = applications.Select(app => new ApplicationViewModel
            {
                Application = app
            });
            return View(viewModels);
        }

        public IActionResult Create()
        {
            var application = new Application
            {
                ApplicationDate = DateTime.Now,
                Status = "Pending",
                Student = new Student
                {
                    StudentID = 0,
                    FirstName = "Unknown",
                    LastName = "Student",
                    Email = "unknown@student.com",
                    DateOfBirth = DateTime.Now,
                    Gender = "Unknown",
                    NationalID = "Unknown",
                    School = new School
                    {
                        SchoolID = 0,
                        SchoolName = "Unknown",
                        Email = "unknown@school.com",
                        Description = "School not found",
                        Majors = new List<Major>(),
                        Students = new List<Student>()
                    },
                    Applications = new List<Application>()
                },
                Major = new Major
                {
                    MajorID = 0,
                    Name = "Unknown",
                    Description = "Major not found",
                    SchoolID = 0,
                    School = new School
                    {
                        SchoolID = 0,
                        SchoolName = "Unknown",
                        Email = "unknown@school.com",
                        Description = "School not found",
                        Majors = new List<Major>(),
                        Students = new List<Student>()
                    },
                    Applications = new List<Application>()
                }
            };

            var students = _studentService.GetAllStudents();
            var majors = _majorService.GetAllMajors();
            _logger.LogInformation($"[DEBUG] Students count: {students.Count()} | Majors count: {majors.Count()}");

            var viewModel = new ApplicationViewModel
            {
                Application = application,
                Students = students,
                Majors = majors
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Create(ApplicationViewModel viewModel)
        {
            if (viewModel.Application == null)
            {
                ModelState.AddModelError("", "Application data is required");
                viewModel.Students = _studentService.GetAllStudents();
                viewModel.Majors = _majorService.GetAllMajors();
                return View(viewModel);
            }

            try
            {
                // Set default values
                viewModel.Application.ApplicationDate = DateTime.Now;
                if (string.IsNullOrEmpty(viewModel.Application.Status))
                {
                    viewModel.Application.Status = "Pending";
                }

                // Create the application
                var applicationId = _applicationService.CreateApplication(viewModel.Application);
                if (applicationId > 0)
                {
                    try
                    {
                        // Create a payment for the application
                        var payment = new Payment
                        {
                            ApplicationID = applicationId,
                            Amount = 1000.00m, // Default application fee
                            PaymentDate = DateTime.Now,
                            Status = "Pending"
                        };

                        _paymentService.CreatePayment(payment);
                        TempData["SuccessMessage"] = "Application submitted successfully. Please complete the payment to finalize your application.";
                    }
                    catch (Exception ex)
                    {
                        // If payment creation fails, we should still show the application details
                        // but inform the user about the payment issue
                        TempData["WarningMessage"] = "Application submitted successfully, but there was an issue creating the payment. Please contact support.";
                        _logger.LogError(ex, "Error creating payment for application {ApplicationId}", applicationId);
                    }
                    
                    return RedirectToAction(nameof(Details), new { id = applicationId });
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create application");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating application: {ex.Message}");
            }

            // If we get here, something went wrong
            viewModel.Students = _studentService.GetAllStudents();
            viewModel.Majors = _majorService.GetAllMajors();
            return View(viewModel);
        }

        public IActionResult Edit(int id)
        {
            try
            {
                var application = _applicationService.GetApplicationWithDetails(id);
                if (application == null)
                {
                    TempData["ErrorMessage"] = "Application not found.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = new ApplicationViewModel
                {
                    Application = application,
                    Students = _studentService.GetAllStudents(),
                    Majors = _majorService.GetAllMajors()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error retrieving application: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationViewModel viewModel)
        {
            try
            {
                if (viewModel.Application == null)
                {
                    ModelState.AddModelError("", "Application data is required");
                    viewModel.Students = _studentService.GetAllStudents();
                    viewModel.Majors = _majorService.GetAllMajors();
                    return View(viewModel);
                }

                // Get the existing application to preserve navigation properties
                var existingApplication = _applicationService.GetApplicationWithDetails(viewModel.Application.ApplicationID);
                if (existingApplication == null)
                {
                    TempData["ErrorMessage"] = "Application not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Ensure required fields are set
                if (string.IsNullOrEmpty(viewModel.Application.Status))
                {
                    viewModel.Application.Status = "Pending";
                }

                if (ModelState.IsValid)
                {
                    // Preserve navigation properties
                    viewModel.Application.Student = existingApplication.Student;
                    viewModel.Application.Major = existingApplication.Major;
                    viewModel.Application.Payment = existingApplication.Payment;

                    // Preserve the original application date if not provided
                    if (viewModel.Application.ApplicationDate == default)
                    {
                        viewModel.Application.ApplicationDate = existingApplication.ApplicationDate;
                    }

                    _applicationService.UpdateApplication(viewModel.Application);
                    TempData["SuccessMessage"] = "Application updated successfully.";
                    return RedirectToAction(nameof(Index));
                }

                // If we get here, something went wrong
                viewModel.Students = _studentService.GetAllStudents();
                viewModel.Majors = _majorService.GetAllMajors();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating application: {ex.Message}";
                viewModel.Students = _studentService.GetAllStudents();
                viewModel.Majors = _majorService.GetAllMajors();
                return View(viewModel);
            }
        }

        public IActionResult Details(int id)
        {
            try
            {
                var application = _applicationService.GetApplicationWithDetails(id);
                if (application == null)
                {
                    TempData["ErrorMessage"] = "Application not found.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = new ApplicationViewModel
                {
                    Application = application,
                    Students = _studentService.GetAllStudents(),
                    Majors = _majorService.GetAllMajors()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error retrieving application details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
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
            try
            {
                _applicationService.DeleteApplication(id);
                TempData["SuccessMessage"] = "Application deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting application: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}