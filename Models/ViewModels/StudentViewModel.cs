using System.Collections.Generic;
using UniversityApplicationSystem.Models;

namespace UniversityApplicationSystem.Models.ViewModels
{
    public class StudentViewModel
    {
        public Student? Student { get; set; }
        public IEnumerable<School>? Schools { get; set; }
    }
}