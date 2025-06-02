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
                SELECT m.*, s.*, a.*, st.*
                FROM Major m
                LEFT JOIN School s ON m.SchoolID = s.SchoolID
                LEFT JOIN Application a ON m.MajorID = a.MajorID
                LEFT JOIN Student st ON a.StudentID = st.StudentID
                WHERE m.MajorID = @ID";
            
            var parameters = new[] { new MySqlParameter("@ID", id) };
            var result = _dbService.ExecuteQuery(query, parameters);
            
            var major = result.ToMajors().FirstOrDefault();
            if (major != null)
            {
                major.School = result.ToSchools().FirstOrDefault();
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
            string query = @"INSERT INTO Major (MajorName, Description, SchoolID, Capacity, DurationYears) 
                           VALUES (@MajorName, @Description, @SchoolID, @Capacity, @DurationYears)";
            
            var parameters = new[]
            {
                new MySqlParameter("@MajorName", major.MajorName),
                new MySqlParameter("@Description", major.Description),
                new MySqlParameter("@SchoolID", major.SchoolID),
                new MySqlParameter("@Capacity", major.Capacity ?? (object)DBNull.Value),
                new MySqlParameter("@DurationYears", major.DurationYears ?? (object)DBNull.Value)
            };

            return _dbService.ExecuteNonQuery(query, parameters);
        }

        public void UpdateMajor(Major major)
        {
            string query = @"UPDATE Major 
                           SET MajorName = @MajorName,
                               Description = @Description,
                               SchoolID = @SchoolID,
                               Capacity = @Capacity,
                               DurationYears = @DurationYears
                           WHERE MajorID = @ID";
            
            var parameters = new[]
            {
                new MySqlParameter("@ID", major.MajorID),
                new MySqlParameter("@MajorName", major.MajorName),
                new MySqlParameter("@Description", major.Description),
                new MySqlParameter("@SchoolID", major.SchoolID),
                new MySqlParameter("@Capacity", major.Capacity ?? (object)DBNull.Value),
                new MySqlParameter("@DurationYears", major.DurationYears ?? (object)DBNull.Value)
            };

            _dbService.ExecuteNonQuery(query, parameters);
        }

        public void DeleteMajor(int id)
        {
            // First delete all applications associated with this major
            string deleteApplicationsQuery = "DELETE FROM Application WHERE MajorID = @ID";
            var parameters = new[] { new MySqlParameter("@ID", id) };
            _dbService.ExecuteNonQuery(deleteApplicationsQuery, parameters);

            // Then delete the major
            string deleteMajorQuery = "DELETE FROM Major WHERE MajorID = @ID";
            _dbService.ExecuteNonQuery(deleteMajorQuery, parameters);
        }
    }
}