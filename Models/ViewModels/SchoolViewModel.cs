using System.Collections.Generic;
using UniversityApplicationSystem.Models;

namespace UniversityApplicationSystem.Models.ViewModels
{
    public class SchoolViewModel
    {
        public School? School { get; set; }
        public IEnumerable<School>? Schools { get; set; }
        public IEnumerable<Major>? Majors { get; set; }
    }
}