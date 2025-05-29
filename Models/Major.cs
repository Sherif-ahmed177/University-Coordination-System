using System.Collections.Generic;

namespace UniversityApplicationSystem.Models
{
    public class Major
    {
        public int MajorID { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int SchoolID { get; set; }
        public int? Capacity { get; set; }
        public int? DurationYears { get; set; }
        
        // Navigation properties
        public School? School { get; set; }
        public ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}