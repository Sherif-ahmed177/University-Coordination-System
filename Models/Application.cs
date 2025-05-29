using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityApplicationSystem.Models
{
    public class Application
    {
        [Key]
        public int ApplicationID { get; set; }

        public int? StudentID { get; set; }
        public int? MajorID { get; set; }

        [Required]
        public DateTime ApplicationDate { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        public decimal? Grade { get; set; }
        
        // Navigation properties
        public Student? Student { get; set; }
        public Major? Major { get; set; }
        public Payment? Payment { get; set; }
    }
}