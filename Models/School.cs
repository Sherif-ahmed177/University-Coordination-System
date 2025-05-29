using System.Collections.Generic;

namespace UniversityApplicationSystem.Models
{
    public class School
    {
        public int? SchoolID { get; set; }
        public required string SchoolName { get; set; }
        public required string Email { get; set; }
        public int? TotalScales { get; set; }
        public decimal? MinRequiredGrade { get; set; }
        public int? EstablishedYear { get; set; }
        public required string Description { get; set; }
        
        // Navigation properties - removed required keyword
        public ICollection<Major> Majors { get; set; } = new List<Major>();
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}