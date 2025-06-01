using System.Data;
using MySql.Data.MySqlClient;
using UniversityApplicationSystem.Models;

namespace UniversityApplicationSystem.Services
{
    public class PaymentService
    {
        private readonly DatabaseService _dbService;

        public PaymentService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public IEnumerable<Payment> GetPayments()
        {
            string query = @"
                SELECT p.*, a.*, s.*, m.*, sc.*
                FROM Payment p
                LEFT JOIN Application a ON p.ApplicationID = a.ApplicationID
                LEFT JOIN Student s ON a.StudentID = s.StudentID
                LEFT JOIN Major m ON a.MajorID = m.MajorID
                LEFT JOIN School sc ON m.SchoolID = sc.SchoolID
                ORDER BY p.PaymentDate DESC";
            
            var result = _dbService.ExecuteQuery(query);
            return result.ToPayments();
        }

        public Payment? GetPayment(int id)
        {
            string query = @"
                SELECT p.*, a.*, s.*, m.*, sc.*
                FROM Payment p
                LEFT JOIN Application a ON p.ApplicationID = a.ApplicationID
                LEFT JOIN Student s ON a.StudentID = s.StudentID
                LEFT JOIN Major m ON a.MajorID = m.MajorID
                LEFT JOIN School sc ON m.SchoolID = sc.SchoolID
                WHERE p.PaymentID = @ID";
            
            var parameters = new[] { new MySqlParameter("@ID", id) };
            var result = _dbService.ExecuteQuery(query, parameters);
            return result.ToPayments().FirstOrDefault();
        }

        public Payment? GetPaymentWithDetails(int id)
        {
            string query = @"
                SELECT p.*, 
                       a.ApplicationID, a.StudentID, a.MajorID, a.ApplicationDate, a.Status, a.Grade,
                       s.StudentID, s.FirstName, s.LastName, s.Email, s.DateOfBirth, s.Gender, s.NationalID, s.SchoolID,
                       m.MajorID, m.Name, m.Description, m.SchoolID, m.Capacity, m.DurationYears,
                       sc.SchoolID, sc.SchoolName, sc.Email, sc.Description, sc.TotalScales, sc.MinRequiredGrade, sc.EstablishedYear
                FROM Payment p
                LEFT JOIN Application a ON p.ApplicationID = a.ApplicationID
                LEFT JOIN Student s ON a.StudentID = s.StudentID
                LEFT JOIN Major m ON a.MajorID = m.MajorID
                LEFT JOIN School sc ON m.SchoolID = sc.SchoolID
                WHERE p.PaymentID = @ID";
            
            var parameters = new[] { new MySqlParameter("@ID", id) };
            var result = _dbService.ExecuteQuery(query, parameters);
            
            var payment = result.ToPayments().FirstOrDefault();
            if (payment != null)
            {
                payment.Application = result.ToApplications().FirstOrDefault() ?? new Application
                {
                    ApplicationID = payment.ApplicationID,
                    StudentID = 0,
                    MajorID = 0,
                    ApplicationDate = DateTime.Now,
                    Status = "Unknown",
                    Student = new Student
                    {
                        StudentID = 0,
                        FirstName = "Unknown",
                        LastName = "Student",
                        Email = "unknown@student.com",
                        DateOfBirth = DateTime.Now,
                        Gender = "Unknown",
                        NationalID = "Unknown",
                        School = new School
                        {
                            SchoolID = 0,
                            SchoolName = "Unknown",
                            Email = "unknown@school.com",
                            Description = "School not found",
                            Majors = new List<Major>(),
                            Students = new List<Student>()
                        },
                        Applications = new List<Application>()
                    },
                    Major = new Major
                    {
                        MajorID = 0,
                        Name = "Unknown",
                        Description = "Major not found",
                        SchoolID = 0,
                        School = new School
                        {
                            SchoolID = 0,
                            SchoolName = "Unknown",
                            Email = "unknown@school.com",
                            Description = "School not found",
                            Majors = new List<Major>(),
                            Students = new List<Student>()
                        },
                        Applications = new List<Application>()
                    },
                    Payment = payment
                };
            }
            return payment;
        }

        public IEnumerable<Payment> GetAllPayments()
        {
            string query = @"
                SELECT p.*, 
                       a.ApplicationID, a.StudentID, a.MajorID, a.ApplicationDate, a.Status, a.Grade,
                       s.StudentID, s.FirstName, s.LastName, s.Email, s.DateOfBirth, s.Gender, s.NationalID, s.SchoolID,
                       m.MajorID, m.Name, m.Description, m.SchoolID, m.Capacity, m.DurationYears,
                       sc.SchoolID, sc.SchoolName, sc.Email, sc.Description, sc.TotalScales, sc.MinRequiredGrade, sc.EstablishedYear
                FROM Payment p
                LEFT JOIN Application a ON p.ApplicationID = a.ApplicationID
                LEFT JOIN Student s ON a.StudentID = s.StudentID
                LEFT JOIN Major m ON a.MajorID = m.MajorID
                LEFT JOIN School sc ON m.SchoolID = sc.SchoolID";
            
            var result = _dbService.ExecuteQuery(query);
            return result.ToPayments();
        }

        public int CreatePayment(Payment payment)
        {
            string query = @"INSERT INTO Payment (ApplicationID, Amount, PaymentDate, Status, TransactionID) 
                           VALUES (@ApplicationID, @Amount, @PaymentDate, @Status, @TransactionID)";
            
            var parameters = new[]
            {
                new MySqlParameter("@ApplicationID", payment.ApplicationID),
                new MySqlParameter("@Amount", payment.Amount),
                new MySqlParameter("@PaymentDate", payment.PaymentDate),
                new MySqlParameter("@Status", payment.Status),
                new MySqlParameter("@TransactionID", payment.TransactionID ?? (object)DBNull.Value)
            };

            return _dbService.ExecuteNonQuery(query, parameters);
        }

        public void UpdatePayment(Payment payment)
        {
            string query = @"UPDATE Payment 
                           SET ApplicationID = @ApplicationID,
                               Amount = @Amount,
                               PaymentDate = @PaymentDate,
                               Status = @Status,
                               TransactionID = @TransactionID
                           WHERE PaymentID = @ID";
            
            var parameters = new[]
            {
                new MySqlParameter("@ID", payment.PaymentID),
                new MySqlParameter("@ApplicationID", payment.ApplicationID),
                new MySqlParameter("@Amount", payment.Amount),
                new MySqlParameter("@PaymentDate", payment.PaymentDate),
                new MySqlParameter("@Status", payment.Status),
                new MySqlParameter("@TransactionID", payment.TransactionID ?? (object)DBNull.Value)
            };

            _dbService.ExecuteNonQuery(query, parameters);
        }

        public void UpdatePaymentStatus(int id, string status, string transactionId = null)
        {
            string query = @"UPDATE Payment 
                           SET Status = @Status
                           WHERE PaymentID = @ID";
            
            var parameters = new[]
            {
                new MySqlParameter("@ID", id),
                new MySqlParameter("@Status", status)
            };

            _dbService.ExecuteNonQuery(query, parameters);
        }

        public void DeletePayment(int id)
        {
            string query = "DELETE FROM Payment WHERE PaymentID = @ID";
            var parameters = new[] { new MySqlParameter("@ID", id) };
            _dbService.ExecuteNonQuery(query, parameters);
        }
    }
} 