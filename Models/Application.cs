using System;

namespace UniversityApplicationSystem.Models
{
    public class Application
    {
        public int ApplicationID { get; set; }
        public int StudentID { get; set; }
        public int MajorID { get; set; }
        public DateTime ApplicationDate { get; set; }
        public required string Status { get; set; } // Pending, Approved, Rejected, Waitlisted
        public decimal? Grade { get; set; }
        public string? Notes { get; set; }
        
        // Navigation properties
        public required Student Student { get; set; }
        public required Major Major { get; set; }
        public required Payment Payment { get; set; }
    }
}