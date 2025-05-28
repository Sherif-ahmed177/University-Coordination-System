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
            string query = "SELECT * FROM Application WHERE ApplicationID = @ID";
            var parameters = new[] { new MySqlParameter("@ID", id) };
            var result = _dbService.ExecuteQuery(query, parameters);
            return result.ToApplications().FirstOrDefault();
        }

        public Application? GetApplicationWithDetails(int id)
        {
            string query = @"
                SELECT a.*, s.*, m.*, p.* 
                FROM Application a
                JOIN Student s ON a.StudentID = s.StudentID
                JOIN Major m ON a.MajorID = m.MajorID
                LEFT JOIN Payment p ON a.ApplicationID = p.ApplicationID
                WHERE a.ApplicationID = @ID";
            
            var parameters = new[] { new MySqlParameter("@ID", id) };
            var result = _dbService.ExecuteQuery(query, parameters);
            
            var application = result.ToApplications().FirstOrDefault();
            if (application != null)
            {
                application.Student = result.ToStudents().FirstOrDefault() ?? new Student
                {
                    StudentID = application.StudentID,
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
                };
                application.Major = result.ToMajors().FirstOrDefault() ?? new Major
                {
                    MajorID = application.MajorID,
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
            string query = "SELECT * FROM Application";
            var result = _dbService.ExecuteQuery(query);
            return result.ToApplications();
        }

        public int CreateApplication(Application application)
        {
            string query = @"INSERT INTO Application (StudentID, MajorID, ApplicationDate, Status, Grade, Notes) 
                           VALUES (@StudentID, @MajorID, @ApplicationDate, @Status, @Grade, @Notes)";
            
            var parameters = new[]
            {
                new MySqlParameter("@StudentID", application.StudentID),
                new MySqlParameter("@MajorID", application.MajorID),
                new MySqlParameter("@ApplicationDate", application.ApplicationDate),
                new MySqlParameter("@Status", application.Status),
                new MySqlParameter("@Grade", application.Grade ?? (object)DBNull.Value),
                new MySqlParameter("@Notes", application.Notes ?? (object)DBNull.Value)
            };

            return _dbService.ExecuteNonQuery(query, parameters);
        }

        public void UpdateApplication(Application application)
        {
            string query = @"UPDATE Application 
                           SET StudentID = @StudentID, 
                               MajorID = @MajorID, 
                               ApplicationDate = @ApplicationDate, 
                               Status = @Status, 
                               Grade = @Grade, 
                               Notes = @Notes 
                           WHERE ApplicationID = @ID";
            
            var parameters = new[]
            {
                new MySqlParameter("@ID", application.ApplicationID),
                new MySqlParameter("@StudentID", application.StudentID),
                new MySqlParameter("@MajorID", application.MajorID),
                new MySqlParameter("@ApplicationDate", application.ApplicationDate),
                new MySqlParameter("@Status", application.Status),
                new MySqlParameter("@Grade", application.Grade ?? (object)DBNull.Value),
                new MySqlParameter("@Notes", application.Notes ?? (object)DBNull.Value)
            };

            _dbService.ExecuteNonQuery(query, parameters);
        }

        public void UpdateApplicationStatus(int id, string status, string changedBy)
        {
            string query = @"UPDATE Application 
                           SET Status = @Status, 
                               Notes = CONCAT(COALESCE(Notes, ''), '\nStatus changed to ', @Status, ' by ', @ChangedBy, ' on ', NOW()) 
                           WHERE ApplicationID = @ID";
            
            var parameters = new[]
            {
                new MySqlParameter("@ID", id),
                new MySqlParameter("@Status", status),
                new MySqlParameter("@ChangedBy", changedBy)
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