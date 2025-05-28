using Microsoft.AspNetCore.Mvc;
using UniversityApplicationSystem.Services;
using UniversityApplicationSystem.Models.ViewModels;
using System.Data;
using System.Text;

namespace UniversityApplicationSystem.Controllers
{
    public class ReportController : Controller
    {
        private readonly ReportService _reportService;
        private readonly SchoolService _schoolService;

        public ReportController(ReportService reportService, SchoolService schoolService)
        {
            _reportService = reportService;
            _schoolService = schoolService;
        }

        public IActionResult Index(int? schoolId, string? status, DateTime? startDate, DateTime? endDate)
        {
            var viewModel = _reportService.GetReportData(schoolId, status, startDate, endDate);
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Generate(ReportViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Schools = _schoolService.GetAllSchools();
                viewModel.Statuses = new[] { "Pending", "Approved", "Rejected" };
                return View("Index", viewModel);
            }

            var reportData = _reportService.GetReportData(
                viewModel.SelectedSchoolId,
                viewModel.SelectedStatus,
                viewModel.StartDate,
                viewModel.EndDate
            );

            return View("Index", reportData);
        }

        public IActionResult Export(ReportViewModel model)
        {
            var reportData = _reportService.GetReportData(
                model.SelectedSchoolId,
                model.SelectedStatus,
                model.StartDate,
                model.EndDate);
            
            return File(GenerateCsv(reportData.ReportData), "text/csv", "ApplicationsReport.csv");
        }

        private byte[] GenerateCsv(DataTable data)
        {
            var csv = new StringBuilder();
            
            // Add headers
            var headers = data.Columns.Cast<DataColumn>()
                .Select(column => column.ColumnName);
            csv.AppendLine(string.Join(",", headers));

            // Add rows
            foreach (DataRow row in data.Rows)
            {
                var fields = row.ItemArray.Select(field => 
                {
                    if (field == null || field == DBNull.Value)
                        return "";
                        
                    var str = field.ToString() ?? "";
                    if (str.Contains(",") || str.Contains("\"") || str.Contains("\n"))
                        return $"\"{str.Replace("\"", "\"\"")}\"";
                        
                    return str;
                });
                
                csv.AppendLine(string.Join(",", fields));
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }
    }
}