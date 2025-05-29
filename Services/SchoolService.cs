using System.Data;
using MySql.Data.MySqlClient;
using UniversityApplicationSystem.Models;
using Microsoft.Extensions.Logging;

namespace UniversityApplicationSystem.Services
{
    public class SchoolService
    {
        private readonly DatabaseService _dbService;
        private readonly ILogger<SchoolService> _logger;

        public SchoolService(DatabaseService dbService, ILogger<SchoolService> logger)
        {
            _dbService = dbService;
            _logger = logger;
        }

        public School? GetSchool(int id)
        {
            string query = @"
                SELECT s.*, m.*, st.* 
                FROM School s
                LEFT JOIN Major m ON s.SchoolID = m.SchoolID
                LEFT JOIN Student st ON s.SchoolID = st.SchoolID
                WHERE s.SchoolID = @ID";
            
            var parameters = new[] { new MySqlParameter("@ID", id) };
            var result = _dbService.ExecuteQuery(query, parameters);
            
            var school = result.ToSchools().FirstOrDefault();
            if (school != null)
            {
                // Only get majors if there are any
                if (result.AsEnumerable().Any(r => !r.IsNull("MajorID")))
                {
                    school.Majors = result.ToMajors().ToList();
                }
                else
                {
                    school.Majors = new List<Major>();
                }

                // Only get students if there are any
                if (result.AsEnumerable().Any(r => !r.IsNull("StudentID")))
                {
                    school.Students = result.ToStudents().ToList();
                }
                else
                {
                    school.Students = new List<Student>();
                }
            }
            return school;
        }

        public School? GetSchoolWithDetails(int id)
        {
            string query = @"
                SELECT s.*, m.*, st.* 
                FROM School s
                LEFT JOIN Major m ON s.SchoolID = m.SchoolID
                LEFT JOIN Student st ON s.SchoolID = st.SchoolID
                WHERE s.SchoolID = @ID";
            
            var parameters = new[] { new MySqlParameter("@ID", id) };
            var result = _dbService.ExecuteQuery(query, parameters);
            
            var school = result.ToSchools().FirstOrDefault();
            if (school != null)
            {
                // Only get majors if there are any
                if (result.AsEnumerable().Any(r => !r.IsNull("MajorID")))
                {
                    school.Majors = result.ToMajors().ToList();
                }
                else
                {
                    school.Majors = new List<Major>();
                }

                // Only get students if there are any
                if (result.AsEnumerable().Any(r => !r.IsNull("StudentID")))
                {
                    school.Students = result.ToStudents().ToList();
                }
                else
                {
                    school.Students = new List<Student>();
                }
            }
            return school;
        }

        public IEnumerable<School> GetAllSchools()
        {
            string query = "SELECT * FROM School";
            var result = _dbService.ExecuteQuery(query);
            return result.ToSchools();
        }

        public int AddSchool(School school)
        {
            _logger.LogInformation("Adding new school: Name={Name}, Email={Email}, Description={Description}, TotalScales={TotalScales}, MinRequiredGrade={MinRequiredGrade}, EstablishedYear={EstablishedYear}",
                school.SchoolName,
                school.Email,
                school.Description,
                school.TotalScales,
                school.MinRequiredGrade,
                school.EstablishedYear);

            string query = @"INSERT INTO School (SchoolName, Email, TotalScales, MinRequiredGrade, EstablishedYear, Description) 
                           VALUES (@SchoolName, @Email, @TotalScales, @MinRequiredGrade, @EstablishedYear, @Description)";
            
            var parameters = new[]
            {
                new MySqlParameter("@SchoolName", school.SchoolName),
                new MySqlParameter("@Email", school.Email),
                new MySqlParameter("@TotalScales", school.TotalScales ?? (object)DBNull.Value),
                new MySqlParameter("@MinRequiredGrade", school.MinRequiredGrade ?? (object)DBNull.Value),
                new MySqlParameter("@EstablishedYear", school.EstablishedYear ?? (object)DBNull.Value),
                new MySqlParameter("@Description", school.Description)
            };

            try
            {
                _logger.LogInformation("Executing SQL query: {Query}", query);
                _logger.LogInformation("Parameters: Name={Name}, Email={Email}, Description={Description}, TotalScales={TotalScales}, MinRequiredGrade={MinRequiredGrade}, EstablishedYear={EstablishedYear}",
                    parameters[0].Value,
                    parameters[1].Value,
                    parameters[2].Value,
                    parameters[3].Value,
                    parameters[4].Value,
                    parameters[5].Value);

                var result = _dbService.ExecuteNonQuery(query, parameters);
                _logger.LogInformation("School added successfully with ID: {Result}", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding school: {ErrorMessage}", ex.Message);
                throw new Exception("Failed to add school: " + ex.Message, ex);
            }
        }

        public void UpdateSchool(School school)
        {
            string query = @"UPDATE School 
                           SET SchoolName = @SchoolName,
                               Email = @Email,
                               TotalScales = @TotalScales,
                               MinRequiredGrade = @MinRequiredGrade,
                               EstablishedYear = @EstablishedYear,
                               Description = @Description
                           WHERE SchoolID = @ID";
            
            var parameters = new[]
            {
                new MySqlParameter("@ID", school.SchoolID),
                new MySqlParameter("@SchoolName", school.SchoolName),
                new MySqlParameter("@Email", school.Email),
                new MySqlParameter("@TotalScales", school.TotalScales ?? (object)DBNull.Value),
                new MySqlParameter("@MinRequiredGrade", school.MinRequiredGrade ?? (object)DBNull.Value),
                new MySqlParameter("@EstablishedYear", school.EstablishedYear ?? (object)DBNull.Value),
                new MySqlParameter("@Description", school.Description)
            };

            _dbService.ExecuteNonQuery(query, parameters);
        }

        public void DeleteSchool(int id)
        {
            string query = "DELETE FROM School WHERE SchoolID = @ID";
            var parameters = new[] { new MySqlParameter("@ID", id) };
            _dbService.ExecuteNonQuery(query, parameters);
        }
    }
}