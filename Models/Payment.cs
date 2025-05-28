using System;

namespace UniversityApplicationSystem.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public int ApplicationID { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public required string Status { get; set; } // Pending, Completed, Failed, Refunded
        public string? TransactionID { get; set; }
        
        // Navigation property
        public required Application Application { get; set; }
    }
}