using System.Collections.Generic;
using UniversityApplicationSystem.Models;

namespace UniversityApplicationSystem.Models.ViewModels
{
    public class ApplicationViewModel
    {
        public Application Application { get; set; } = new();
        public IEnumerable<Student>? Students { get; set; }
        public IEnumerable<Major>? Majors { get; set; }
        public int? SelectedStudentId { get; set; }
        public IEnumerable<School>? Schools { get; set; }
    }
}