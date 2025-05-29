using System;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel.DataAnnotations;
// Add the correct namespace for School if it exists, for example:
// <-- Change this to the actual namespace where School is defined
using UniversityApplicationSystem.Models;

namespace UniversityApplicationSystem.Models.ViewModels
{
    public class ReportViewModel
    {
        [Required(ErrorMessage = "Please select a report type")]
        public string ReportType { get; set; } = string.Empty;

        public List<string> ReportTypes { get; set; } = new();
        public List<Major> Majors { get; set; } = new();
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IEnumerable<School>? Schools { get; set; }
        public IEnumerable<string>? Statuses { get; set; }
        public string? SelectedStatus { get; set; }
        public int? SelectedSchoolId { get; set; }
        public DataTable? ReportData { get; set; }
    }

    public class ReportDataViewModel
    {
        public string Category { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal? TotalAmount { get; set; }
        public List<object> Details { get; set; } = new();
    }
}