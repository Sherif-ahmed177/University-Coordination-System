using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using UniversityApplicationSystem.Models;
using UniversityApplicationSystem.Models.ViewModels;

namespace UniversityApplicationSystem.Services
{
    public class ReportService
    {
        private readonly DatabaseService _databaseService;

        public ReportService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public ReportViewModel GetReportData(int? schoolId, string? status, DateTime? startDate, DateTime? endDate)
        {
            var viewModel = new ReportViewModel
            {
                Schools = GetSchools(),
                Statuses = new[] { "Pending", "Approved", "Rejected" },
                SelectedSchoolId = schoolId,
                SelectedStatus = status,
                StartDate = startDate,
                EndDate = endDate
            };

            var parameters = new List<MySqlParameter>();
            var conditions = new List<string>();

            if (schoolId.HasValue)
            {
                parameters.Add(new MySqlParameter("@SchoolId", schoolId.Value));
                conditions.Add("s.SchoolID = @SchoolId");
            }

            if (!string.IsNullOrEmpty(status))
            {
                parameters.Add(new MySqlParameter("@Status", status));
                conditions.Add("a.Status = @Status");
            }

            if (startDate.HasValue)
            {
                parameters.Add(new MySqlParameter("@StartDate", startDate.Value));
                conditions.Add("a.ApplicationDate >= @StartDate");
            }

            if (endDate.HasValue)
            {
                parameters.Add(new MySqlParameter("@EndDate", endDate.Value));
                conditions.Add("a.ApplicationDate <= @EndDate");
            }

            var whereClause = conditions.Any() ? "WHERE " + string.Join(" AND ", conditions) : "";

            var query = $@"
                SELECT 
                    s.SchoolName,
                    m.Name as MajorName,
                    COUNT(*) as ApplicationCount,
                    SUM(CASE WHEN a.Status = 'Approved' THEN 1 ELSE 0 END) as ApprovedCount,
                    SUM(CASE WHEN a.Status = 'Rejected' THEN 1 ELSE 0 END) as RejectedCount,
                    AVG(a.Grade) as AverageGrade
                FROM Applications a
                JOIN Majors m ON a.MajorID = m.MajorID
                JOIN Schools s ON m.SchoolID = s.SchoolID
                {whereClause}
                GROUP BY s.SchoolName, m.Name
                ORDER BY s.SchoolName, m.Name";

            var dataTable = _databaseService.ExecuteQuery(query, parameters.ToArray());
            viewModel.ReportData = dataTable;

            return viewModel;
        }

        private IEnumerable<School> GetSchools()
        {
            var query = "SELECT * FROM Schools ORDER BY SchoolName";
            var dataTable = _databaseService.ExecuteQuery(query);
            return dataTable.ToSchools();
        }
    }
}