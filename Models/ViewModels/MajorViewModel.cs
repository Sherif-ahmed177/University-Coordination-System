using System.ComponentModel.DataAnnotations;

namespace UniversityApplicationSystem.Models.ViewModels
{
    public class MajorViewModel
    {
        public int SchoolID { get; set; }
        public string SchoolName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Major name is required")]
        [StringLength(100, ErrorMessage = "Major name cannot be longer than 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 6, ErrorMessage = "Duration must be between 1 and 6 years")]
        public int DurationYears { get; set; }
    }
}