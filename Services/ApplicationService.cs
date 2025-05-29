using System.Data;
using MySql.Data.MySqlClient;
using UniversityApplicationSystem.Models;

namespace UniversityApplicationSystem.Services
{
    public class ApplicationService
    {
        private readonly DatabaseService _dbService;

        public ApplicationService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public Application? GetApplication(int id)
        {
            string query = @"
                SELECT a.*, s.*, m.*, sc.*
                FROM Application a
                LEFT JOIN Student s ON a.StudentID = s.StudentID
                LEFT JOIN Major m ON a.MajorID = m.MajorID
                LEFT JOIN School sc ON m.SchoolID = sc.SchoolID
                WHERE a.ApplicationID = @ID";
            
            var parameters = new[] { new MySqlParameter("@ID", id) };
            var result = _dbService.ExecuteQuery(query, parameters);
            return result.ToApplications().FirstOrDefault();
        }

        public Application? GetApplicationWithDetails(int id)
        {
            string query = @"
                SELECT a.*, s.*, m.*, sc.*, p.* 
                FROM Application a
                LEFT JOIN Student s ON a.StudentID = s.StudentID
                LEFT JOIN Major m ON a.MajorID = m.MajorID
                LEFT JOIN School sc ON m.SchoolID = sc.SchoolID
                LEFT JOIN Payment p ON a.ApplicationID = p.ApplicationID
                WHERE a.ApplicationID = @ID";
            
            var parameters = new[] { new MySqlParameter("@ID", id) };
            var result = _dbService.ExecuteQuery(query, parameters);
            
            var application = result.ToApplications().FirstOrDefault();
            if (application != null)
            {
                application.Student = result.ToStudents().FirstOrDefault() ?? new Student
                {
                    StudentID = application.StudentID ?? 0,
                    FirstName = "Unknown",
                    LastName = "Student",
                    Email = "unknown@student.com",
                    DateOfBirth = DateTime.Now,
                    Gender = "Unknown",
                    NationalID = "Unknown",
                    SchoolID = null,
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
                };
                application.Major = result.ToMajors().FirstOrDefault() ?? new Major
                {
                    MajorID = application.MajorID ?? 0,
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
                };
                application.Payment = result.ToPayments().FirstOrDefault() ?? new Payment
                {
                    ApplicationID = application.ApplicationID,
                    Status = "Pending",
                    Amount = 0,
                    PaymentDate = DateTime.Now,
                    Application = application
                };
            }
            return application;
        }

        public IEnumerable<Application> GetAllApplications()
        {
            string query = @"
                SELECT a.*, s.*, m.*, sc.*
                FROM Application a
                LEFT JOIN Student s ON a.StudentID = s.StudentID
                LEFT JOIN Major m ON a.MajorID = m.MajorID
                LEFT JOIN School sc ON m.SchoolID = sc.SchoolID";
            
            var result = _dbService.ExecuteQuery(query);
            return result.ToApplications();
        }

        public int CreateApplication(Application application)
        {
            string query = @"INSERT INTO Application (StudentID, MajorID, ApplicationDate, Status, Grade) 
                           VALUES (@StudentID, @MajorID, @ApplicationDate, @Status, @Grade);
                           SELECT LAST_INSERT_ID();";
            
            var parameters = new[]
            {
                new MySqlParameter("@StudentID", application.StudentID ?? (object)DBNull.Value),
                new MySqlParameter("@MajorID", application.MajorID ?? (object)DBNull.Value),
                new MySqlParameter("@ApplicationDate", application.ApplicationDate),
                new MySqlParameter("@Status", application.Status),
                new MySqlParameter("@Grade", application.Grade ?? (object)DBNull.Value)
            };

            var result = _dbService.ExecuteScalar(query, parameters);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public void UpdateApplication(Application application)
        {
            string query = @"UPDATE Application 
                           SET StudentID = @StudentID,
                               MajorID = @MajorID,
                               ApplicationDate = @ApplicationDate,
                               Status = @Status,
                               Grade = @Grade
                           WHERE ApplicationID = @ID";
            
            var parameters = new[]
            {
                new MySqlParameter("@ID", application.ApplicationID),
                new MySqlParameter("@StudentID", application.StudentID ?? (object)DBNull.Value),
                new MySqlParameter("@MajorID", application.MajorID ?? (object)DBNull.Value),
                new MySqlParameter("@ApplicationDate", application.ApplicationDate),
                new MySqlParameter("@Status", application.Status),
                new MySqlParameter("@Grade", application.Grade ?? (object)DBNull.Value)
            };

            _dbService.ExecuteNonQuery(query, parameters);
        }

        public void UpdateApplicationStatus(int id, string status, string changedBy)
        {
            string query = @"UPDATE Application 
                           SET Status = @Status
                           WHERE ApplicationID = @ID";
            
            var parameters = new[]
            {
                new MySqlParameter("@ID", id),
                new MySqlParameter("@Status", status)
            };

            _dbService.ExecuteNonQuery(query, parameters);
        }

        public void DeleteApplication(int id)
        {
            string query = "DELETE FROM Application WHERE ApplicationID = @ID";
            var parameters = new[] { new MySqlParameter("@ID", id) };
            _dbService.ExecuteNonQuery(query, parameters);
        }
    }
}