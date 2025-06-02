using Microsoft.AspNetCore.Mvc;
using UniversityApplicationSystem.Services;
using UniversityApplicationSystem.Models.ViewModels;
using System.Data;

namespace UniversityApplicationSystem.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationService _applicationService;
        private readonly MajorService _majorService;
        private readonly PaymentService _paymentService;
        private readonly StudentService _studentService;

        public ReportController(
            ApplicationService applicationService,
            MajorService majorService,
            PaymentService paymentService,
            StudentService studentService)
        {
            _applicationService = applicationService;
            _majorService = majorService;
            _paymentService = paymentService;
            _studentService = studentService;
        }

        public IActionResult Index()
        {
            var viewModel = new ReportViewModel
            {
                ReportTypes = new List<string>
                {
                    "Applications by Status",
                    "Applications by Major",
                    "Payment Status",
                    "Student Enrollment"
                },
                Majors = _majorService.GetAllMajors().ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult GenerateReport(ReportViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Majors = _majorService.GetAllMajors().ToList();
                return View("Index", model);
            }

            switch (model.ReportType)
            {
                case "Applications by Status":
                    return ApplicationsByStatusReport(model);
                case "Applications by Major":
                    return ApplicationsByMajorReport(model);
                case "Payment Status":
                    return PaymentStatusReport(model);
                case "Student Enrollment":
                    return StudentEnrollmentReport(model);
                default:
                    ModelState.AddModelError("", "Invalid report type selected");
                    model.Majors = _majorService.GetAllMajors().ToList();
                    return View("Index", model);
            }
        }

        private IActionResult ApplicationsByStatusReport(ReportViewModel model)
        {
            var applications = _applicationService.GetAllApplications();
            var reportData = applications
                .GroupBy(a => a.Status)
                .Select(g => new ReportDataViewModel
                {
                    Category = g.Key,
                    Count = g.Count(),
                    Details = g.Select(a => new ApplicationViewModel { Application = a }).Cast<object>().ToList()
                })
                .ToList();

            ViewBag.ReportTitle = "Applications by Status";
            ViewBag.ReportType = "Applications by Status";
            return View("ReportResult", reportData);
        }

        private IActionResult ApplicationsByMajorReport(ReportViewModel model)
        {
            var applications = _applicationService.GetAllApplications();
            var reportData = applications
                .GroupBy(a => a.Major?.MajorName ?? "Unknown")
                .Select(g => new ReportDataViewModel
                {
                    Category = g.Key,
                    Count = g.Count(),
                    Details = g.Select(a => new ApplicationViewModel { Application = a }).Cast<object>().ToList()
                })
                .ToList();

            ViewBag.ReportTitle = "Applications by Major";
            ViewBag.ReportType = "Applications by Major";
            return View("ReportResult", reportData);
        }

        private IActionResult PaymentStatusReport(ReportViewModel model)
        {
            var payments = _paymentService.GetPayments();
            var reportData = payments
                .GroupBy(p => p.Status)
                .Select(g => new ReportDataViewModel
                {
                    Category = g.Key,
                    Count = g.Count(),
                    TotalAmount = g.Sum(p => p.Amount),
                    Details = g.Select(p => new PaymentViewModel { Payment = p }).Cast<object>().ToList()
                })
                .ToList();

            ViewBag.ReportTitle = "Payment Status Report";
            ViewBag.ReportType = "Payment Status";
            return View("ReportResult", reportData);
        }

        private IActionResult StudentEnrollmentReport(ReportViewModel model)
        {
            var students = _studentService.GetAllStudents();
            var reportData = students
                .GroupBy(s => s.School?.SchoolName ?? "Unknown")
                .Select(g => new ReportDataViewModel
                {
                    Category = g.Key,
                    Count = g.Count(),
                    Details = g.Select(s => new StudentViewModel { Student = s }).Cast<object>().ToList()
                })
                .ToList();

            ViewBag.ReportTitle = "Student Enrollment by School";
            ViewBag.ReportType = "Student Enrollment";
            return View("ReportResult", reportData);
        }
    }
}