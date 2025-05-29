using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniversityApplicationSystem.Models
{
    public class Student
    {
        public int StudentID { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public string NationalID { get; set; } = string.Empty;

        public int? SchoolID { get; set; }

        // Make these optional for creation
        public School? School { get; set; }
        public ICollection<Application>? Applications { get; set; }
    }
}