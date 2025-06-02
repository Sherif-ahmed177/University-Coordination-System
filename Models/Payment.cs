using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityApplicationSystem.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }

        [Required]
        public int ApplicationID { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive.")]
        public decimal Amount { get; set; }

        [Required]
        [StatusValidation]
        public string Status { get; set; } = "Pending";

        [PaymentDateNotInFuture]
        public DateTime PaymentDate { get; set; }

        public string? TransactionID { get; set; }

        // Navigation property
        public Application? Application { get; set; }
    }

    public class StatusValidationAttribute : ValidationAttribute
    {
        private static readonly string[] AllowedStatuses = { "Pending", "Completed", "Failed", "Refunded" };
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string status && AllowedStatuses.Contains(status))
                return ValidationResult.Success;
            return new ValidationResult($"Status must be one of: {string.Join(", ", AllowedStatuses)}");
        }
    }

    public class PaymentDateNotInFutureAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date && date > DateTime.Now)
                return new ValidationResult("Payment date cannot be in the future.");
            return ValidationResult.Success;
        }
    }
}