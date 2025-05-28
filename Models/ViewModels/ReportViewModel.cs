using System;
using System.Collections.Generic;
using System.Data;
// Add the correct namespace for School if it exists, for example:
// <-- Change this to the actual namespace where School is defined
using UniversityApplicationSystem.Models;

namespace UniversityApplicationSystem.Models.ViewModels
{
    public class ReportViewModel
    {
        public IEnumerable<School>? Schools { get; set; }
        public IEnumerable<string>? Statuses { get; set; }
        public string? SelectedStatus { get; set; }
        public int? SelectedSchoolId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DataTable? ReportData { get; set; }
    }
}