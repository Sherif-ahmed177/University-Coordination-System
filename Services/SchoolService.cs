using System.Data;
using MySql.Data.MySqlClient;
using UniversityApplicationSystem.Models;

namespace UniversityApplicationSystem.Services
{
    public class SchoolService
    {
        private readonly DatabaseService _dbService;

        public SchoolService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public School? GetSchool(int id)
        {
            string query = "SELECT * FROM School WHERE SchoolID = @ID";
            var parameters = new[] { new MySqlParameter("@ID", id) };
            var result = _dbService.ExecuteQuery(query, parameters);
            return result.ToSchools().FirstOrDefault();
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
                school.Majors = result.ToMajors().ToList();
                school.Students = result.ToStudents().ToList();
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
            string query = @"INSERT INTO School (SchoolName, Email, TotalScales, MinRequiredGrade, EstablishedYear, Description) 
                           VALUES (@Name, @Email, @TotalScales, @MinRequiredGrade, @EstablishedYear, @Description)";
            
            var parameters = new[]
            {
                new MySqlParameter("@Name", school.SchoolName),
                new MySqlParameter("@Email", school.Email),
                new MySqlParameter("@TotalScales", school.TotalScales ?? (object)DBNull.Value),
                new MySqlParameter("@MinRequiredGrade", school.MinRequiredGrade ?? (object)DBNull.Value),
                new MySqlParameter("@EstablishedYear", school.EstablishedYear ?? (object)DBNull.Value),
                new MySqlParameter("@Description", school.Description)
            };

            return _dbService.ExecuteNonQuery(query, parameters);
        }

        public void UpdateSchool(School school)
        {
            string query = @"UPDATE School 
                           SET SchoolName = @Name, 
                               Email = @Email, 
                               TotalScales = @TotalScales, 
                               MinRequiredGrade = @MinRequiredGrade, 
                               EstablishedYear = @EstablishedYear, 
                               Description = @Description 
                           WHERE SchoolID = @ID";
            
            var parameters = new[]
            {
                new MySqlParameter("@ID", school.SchoolID),
                new MySqlParameter("@Name", school.SchoolName),
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