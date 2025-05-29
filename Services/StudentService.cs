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
            // First get the student with school details
            string studentQuery = @"
                SELECT s.*, sc.*
                FROM Student s
                LEFT JOIN School sc ON s.SchoolID = sc.SchoolID
                WHERE s.StudentID = @ID";
            
            var parameters = new[] { new MySqlParameter("@ID", id) };
            var studentResult = _dbService.ExecuteQuery(studentQuery, parameters);
            var student = studentResult.ToStudents().FirstOrDefault();

            if (student != null)
            {
                // Then get the applications with major and school details
                string applicationsQuery = @"
                    SELECT a.*, m.*, s.*, sc.*
                    FROM Application a
                    LEFT JOIN Major m ON a.MajorID = m.MajorID
                    LEFT JOIN Student s ON a.StudentID = s.StudentID
                    LEFT JOIN School sc ON m.SchoolID = sc.SchoolID
                    WHERE a.StudentID = @ID";
                
                var applicationsResult = _dbService.ExecuteQuery(applicationsQuery, parameters);
                student.Applications = applicationsResult.ToApplications().ToList();
            }

            return student;
        }

        public IEnumerable<Student> GetAllStudents()
        {
            string query = @"
                SELECT s.*, sc.*
                FROM Student s
                LEFT JOIN School sc ON s.SchoolID = sc.SchoolID";
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