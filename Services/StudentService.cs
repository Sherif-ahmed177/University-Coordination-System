using System.Data;
using MySql.Data.MySqlClient;
using UniversityApplicationSystem.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace UniversityApplicationSystem.Services
{
    public class StudentService
    {
        private readonly DatabaseService _dbService;
        private readonly ILogger<StudentService> _logger;

        public StudentService(DatabaseService dbService, ILogger<StudentService> logger)
        {
            _dbService = dbService;
            _logger = logger;
        }

        public Student? GetStudent(int id)
        {
            string query = "SELECT * FROM Student WHERE StudentID = @ID";
            var parameters = new[] { new MySqlParameter("@ID", id) };
            var result = _dbService.ExecuteQuery(query, parameters);
            return result.ToStudents().FirstOrDefault();
        }

        public Student? GetStudentWithDetails(int id)
        {
            _logger.LogInformation("Getting student details for ID: {ID}", id);

            // First get the student with their school
            string query = @"
                SELECT s.*, sc.*
                FROM Student s
                LEFT JOIN School sc ON s.SchoolID = sc.SchoolID
                WHERE s.StudentID = @ID";
            
            var parameters = new[] { new MySqlParameter("@ID", id) };
            var result = _dbService.ExecuteQuery(query, parameters);
            
            var student = result.ToStudents().FirstOrDefault();
            if (student == null)
            {
                _logger.LogWarning("Student not found with ID: {ID}", id);
                return null;
            }

            // Get the student's applications with major and school details
            string applicationsQuery = @"
                SELECT 
                    a.ApplicationID, a.StudentID, a.MajorID, a.ApplicationDate, 
                    a.Status, a.Grade,
                    m.Name, m.Description, m.SchoolID,
                    s.SchoolName, s.Email, s.Description as SchoolDescription
                FROM Application a
                LEFT JOIN Major m ON a.MajorID = m.MajorID
                LEFT JOIN School s ON m.SchoolID = s.SchoolID
                WHERE a.StudentID = @ID";
            
            var applicationsResult = _dbService.ExecuteQuery(applicationsQuery, parameters);
            student.Applications = applicationsResult.ToApplications().ToList();

            _logger.LogInformation("Successfully loaded student details for ID: {ID}", id);
            return student;
        }

        public IEnumerable<Student> GetAllStudents()
        {
            string query = "SELECT * FROM Student";
            var result = _dbService.ExecuteQuery(query);
            return result.ToStudents();
        }

        public int AddStudent(Student student)
        {
            _logger.LogInformation("Adding new student: FirstName={FirstName}, LastName={LastName}, Email={Email}, SchoolID={SchoolID}",
                student.FirstName,
                student.LastName,
                student.Email,
                student.SchoolID);

            string query = @"INSERT INTO Student (FirstName, LastName, Email, DateOfBirth, Gender, NationalID, SchoolID) 
                           VALUES (@FirstName, @LastName, @Email, @DateOfBirth, @Gender, @NationalID, @SchoolID)";
            
            var parameters = new[]
            {
                new MySqlParameter("@FirstName", student.FirstName),
                new MySqlParameter("@LastName", student.LastName),
                new MySqlParameter("@Email", student.Email),
                new MySqlParameter("@DateOfBirth", student.DateOfBirth),
                new MySqlParameter("@Gender", student.Gender),
                new MySqlParameter("@NationalID", student.NationalID),
                new MySqlParameter("@SchoolID", student.SchoolID ?? (object)DBNull.Value)
            };

            try
            {
                var result = _dbService.ExecuteNonQuery(query, parameters);
                _logger.LogInformation("Student added successfully with result: {Result}", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding student");
                throw;
            }
        }

        public void UpdateStudent(Student student)
        {
            string query = @"UPDATE Student 
                           SET FirstName = @FirstName, 
                               LastName = @LastName, 
                               Email = @Email, 
                               DateOfBirth = @DateOfBirth, 
                               Gender = @Gender, 
                               NationalID = @NationalID, 
                               SchoolID = @SchoolID 
                           WHERE StudentID = @ID";
            
            var parameters = new[]
            {
                new MySqlParameter("@ID", student.StudentID),
                new MySqlParameter("@FirstName", student.FirstName),
                new MySqlParameter("@LastName", student.LastName),
                new MySqlParameter("@Email", student.Email),
                new MySqlParameter("@DateOfBirth", student.DateOfBirth),
                new MySqlParameter("@Gender", student.Gender),
                new MySqlParameter("@NationalID", student.NationalID),
                new MySqlParameter("@SchoolID", student.SchoolID ?? (object)DBNull.Value)
            };

            _dbService.ExecuteNonQuery(query, parameters);
        }

        public void DeleteStudent(int id)
        {
            string query = "DELETE FROM Student WHERE StudentID = @ID";
            var parameters = new[] { new MySqlParameter("@ID", id) };
            _dbService.ExecuteNonQuery(query, parameters);
        }
    }
}