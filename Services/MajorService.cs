using System.Data;
using MySql.Data.MySqlClient;
using UniversityApplicationSystem.Models;

namespace UniversityApplicationSystem.Services
{
    public class MajorService
    {
        private readonly DatabaseService _dbService;

        public MajorService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public Major? GetMajor(int id)
        {
            string query = "SELECT * FROM Major WHERE MajorID = @ID";
            var parameters = new[] { new MySqlParameter("@ID", id) };
            var result = _dbService.ExecuteQuery(query, parameters);
            return result.ToMajors().FirstOrDefault();
        }

        public Major? GetMajorWithDetails(int id)
        {
            string query = @"
                SELECT m.*, s.*, a.* 
                FROM Major m
                JOIN School s ON m.SchoolID = s.SchoolID
                LEFT JOIN Application a ON m.MajorID = a.MajorID
                WHERE m.MajorID = @ID";
            
            var parameters = new[] { new MySqlParameter("@ID", id) };
            var result = _dbService.ExecuteQuery(query, parameters);
            
            var major = result.ToMajors().FirstOrDefault();
            if (major != null)
            {
                major.School = result.ToSchools().First();
                major.Applications = result.ToApplications().ToList();
            }
            return major;
        }

        public IEnumerable<Major> GetAllMajors()
        {
            string query = "SELECT * FROM Major";
            var result = _dbService.ExecuteQuery(query);
            return result.ToMajors();
        }

        public int AddMajor(Major major)
        {
            string query = @"INSERT INTO Major (Name, SchoolID, Description, Capacity, DurationYears) 
                           VALUES (@Name, @SchoolID, @Description, @Capacity, @DurationYears)";
            
            var parameters = new[]
            {
                new MySqlParameter("@Name", major.Name),
                new MySqlParameter("@SchoolID", major.SchoolID),
                new MySqlParameter("@Description", major.Description),
                new MySqlParameter("@Capacity", major.Capacity ?? (object)DBNull.Value),
                new MySqlParameter("@DurationYears", major.DurationYears)
            };

            return _dbService.ExecuteNonQuery(query, parameters);
        }

        public void UpdateMajor(Major major)
        {
            string query = @"UPDATE Major 
                           SET Name = @Name, 
                               SchoolID = @SchoolID, 
                               Description = @Description, 
                               Capacity = @Capacity, 
                               DurationYears = @DurationYears 
                           WHERE MajorID = @ID";
            
            var parameters = new[]
            {
                new MySqlParameter("@ID", major.MajorID),
                new MySqlParameter("@Name", major.Name),
                new MySqlParameter("@SchoolID", major.SchoolID),
                new MySqlParameter("@Description", major.Description),
                new MySqlParameter("@Capacity", major.Capacity ?? (object)DBNull.Value),
                new MySqlParameter("@DurationYears", major.DurationYears)
            };

            _dbService.ExecuteNonQuery(query, parameters);
        }

        public void DeleteMajor(int id)
        {
            string query = "DELETE FROM Major WHERE MajorID = @ID";
            var parameters = new[] { new MySqlParameter("@ID", id) };
            _dbService.ExecuteNonQuery(query, parameters);
        }
    }
}