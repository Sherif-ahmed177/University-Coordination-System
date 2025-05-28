using System.Collections.Generic;
using UniversityApplicationSystem.Models;

namespace UniversityApplicationSystem.Models.ViewModels
{
    public class MajorViewModel
    {
        public Major? Major { get; set; }
        public IEnumerable<School>? Schools { get; set; }
    }
}