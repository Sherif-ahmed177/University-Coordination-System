using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using UniversityApplicationSystem.Models;
using Microsoft.Extensions.Logging;

namespace UniversityApplicationSystem.Services
{
    public static class DataTableExtensions
    {
        public static IEnumerable<School> ToSchools(this DataTable table)
        {
            return table.AsEnumerable().Select(row => new School
            {
                SchoolID = row.Field<int>("SchoolID"),
                SchoolName = row.Field<string>("SchoolName"),
                Email = row.Field<string>("Email"),
                TotalScales = row.Field<int?>("TotalScales"),
                MinRequiredGrade = row.Field<decimal?>("MinRequiredGrade"),
                EstablishedYear = row.Field<int?>("EstablishedYear"),
                Description = row.Field<string>("Description"),
                Majors = new List<Major>(),
                Students = new List<Student>()
            });
        }

        public static IEnumerable<Student> ToStudents(this DataTable table)
        {
            return table.AsEnumerable().Select(row => new Student
            {
                StudentID = row.Field<int>("StudentID"),
                FirstName = row.Field<string>("FirstName"),
                LastName = row.Field<string>("LastName"),
                Email = row.Field<string>("Email"),
                DateOfBirth = row.Field<DateTime>("DateOfBirth"),
                Gender = row.Field<string>("Gender"),
                NationalIDNumber = row.Field<string>("NationalIDNumber"),
                SchoolID = row.Field<int?>("SchoolID"),
                School = row.Table.Columns.Contains("SchoolName") ? new School
                {
                    SchoolID = row.Field<int?>("SchoolID") ?? 0,
                    SchoolName = row.Field<string>("SchoolName") ?? "Unknown",
                    Email = row.Field<string>("Email") ?? "unknown@school.com",
                    Description = row.Field<string>("Description") ?? "School not found",
                    TotalScales = row.Field<int?>("TotalScales"),
                    MinRequiredGrade = row.Field<decimal?>("MinRequiredGrade"),
                    EstablishedYear = row.Field<int?>("EstablishedYear"),
                    Majors = new List<Major>(),
                    Students = new List<Student>()
                } : null,
                Applications = new List<Application>()
            });
        }

        public static IEnumerable<Major> ToMajors(this DataTable table)
        {
            return table.AsEnumerable().Select(row => new Major
            {
                MajorID = row.Field<int>("MajorID"),
                MajorName = row.Field<string>("MajorName") ?? "Unknown",
                Description = row.Field<string>("Description") ?? "No description",
                SchoolID = row.Field<int>("SchoolID"),
                Capacity = row.IsNull("Capacity") ? null : row.Field<int>("Capacity"),
                DurationYears = row.IsNull("DurationYears") ? 4 : row.Field<int>("DurationYears"),
                School = row.Table.Columns.Contains("SchoolName") ? new School
                {
                    SchoolID = row.Field<int>("SchoolID"),
                    SchoolName = row.Field<string>("SchoolName") ?? "Unknown",
                    Email = row.Field<string>("Email") ?? "unknown@school.com",
                    Description = row.Field<string>("Description") ?? "School not found",
                    TotalScales = row.IsNull("TotalScales") ? null : row.Field<int>("TotalScales"),
                    MinRequiredGrade = row.IsNull("MinRequiredGrade") ? null : row.Field<decimal>("MinRequiredGrade"),
                    EstablishedYear = row.IsNull("EstablishedYear") ? null : row.Field<int>("EstablishedYear"),
                    Majors = new List<Major>(),
                    Students = new List<Student>()
                } : null,
                Applications = new List<Application>()
            });
        }

        public static IEnumerable<Application> ToApplications(this DataTable table)
        {
            return table.AsEnumerable().Select(row => new Application
            {
                ApplicationID = row.IsNull("ApplicationID") ? 0 : row.Field<int>("ApplicationID"),
                StudentID = row.IsNull("StudentID") ? null : row.Field<int>("StudentID"),
                MajorID = row.IsNull("MajorID") ? null : row.Field<int>("MajorID"),
                ApplicationDate = row.IsNull("ApplicationDate") ? DateTime.Now : row.Field<DateTime>("ApplicationDate"),
                Status = row.IsNull("Status") ? "Pending" : row.Field<string>("Status"),
                Grade = row.IsNull("Grade") ? null : row.Field<decimal>("Grade"),
                Student = row.IsNull("StudentID") ? null : new Student
                {
                    StudentID = row.IsNull("StudentID") ? 0 : row.Field<int>("StudentID"),
                    FirstName = row.IsNull("FirstName") ? "Unknown" : row.Field<string>("FirstName"),
                    LastName = row.IsNull("LastName") ? "Student" : row.Field<string>("LastName"),
                    Email = row.IsNull("Email") ? "unknown@student.com" : row.Field<string>("Email"),
                    DateOfBirth = row.IsNull("DateOfBirth") ? DateTime.Now : row.Field<DateTime>("DateOfBirth"),
                    Gender = row.IsNull("Gender") ? "Unknown" : row.Field<string>("Gender"),
                    NationalIDNumber = row.IsNull("NationalIDNumber") ? "Unknown" : row.Field<string>("NationalIDNumber"),
                    SchoolID = row.IsNull("SchoolID") ? null : row.Field<int>("SchoolID"),
                    School = row.IsNull("SchoolName") ? null : new School
                    {
                        SchoolID = row.IsNull("SchoolID") ? 0 : row.Field<int>("SchoolID"),
                        SchoolName = row.IsNull("SchoolName") ? "Unknown" : row.Field<string>("SchoolName"),
                        Email = row.IsNull("Email") ? "unknown@school.com" : row.Field<string>("Email"),
                        Description = row.IsNull("Description") ? "School not found" : row.Field<string>("Description"),
                        TotalScales = row.IsNull("TotalScales") ? null : row.Field<int>("TotalScales"),
                        MinRequiredGrade = row.IsNull("MinRequiredGrade") ? null : row.Field<decimal>("MinRequiredGrade"),
                        EstablishedYear = row.IsNull("EstablishedYear") ? null : row.Field<int>("EstablishedYear"),
                        Majors = new List<Major>(),
                        Students = new List<Student>()
                    },
                    Applications = new List<Application>()
                },
                Major = row.IsNull("MajorID") ? null : new Major
                {
                    MajorID = row.IsNull("MajorID") ? 0 : row.Field<int>("MajorID"),
                    MajorName = row.IsNull("MajorName") ? "Unknown" : row.Field<string>("MajorName"),
                    Description = row.IsNull("Description") ? "No description" : row.Field<string>("Description"),
                    SchoolID = row.IsNull("SchoolID") ? 0 : row.Field<int>("SchoolID"),
                    Capacity = row.IsNull("Capacity") ? null : row.Field<int>("Capacity"),
                    DurationYears = row.IsNull("DurationYears") ? 4 : row.Field<int>("DurationYears"),
                    School = row.IsNull("SchoolName") ? null : new School
                    {
                        SchoolID = row.IsNull("SchoolID") ? 0 : row.Field<int>("SchoolID"),
                        SchoolName = row.IsNull("SchoolName") ? "Unknown" : row.Field<string>("SchoolName"),
                        Email = row.IsNull("Email") ? "unknown@school.com" : row.Field<string>("Email"),
                        Description = row.IsNull("Description") ? "School not found" : row.Field<string>("Description"),
                        TotalScales = row.IsNull("TotalScales") ? null : row.Field<int>("TotalScales"),
                        MinRequiredGrade = row.IsNull("MinRequiredGrade") ? null : row.Field<decimal>("MinRequiredGrade"),
                        EstablishedYear = row.IsNull("EstablishedYear") ? null : row.Field<int>("EstablishedYear"),
                        Majors = new List<Major>(),
                        Students = new List<Student>()
                    },
                    Applications = new List<Application>()
                }
            });
        }

        public static IEnumerable<Payment> ToPayments(this DataTable table)
        {
            return table.AsEnumerable().Select(row => new Payment
            {
                PaymentID = row.IsNull("PaymentID") ? 0 : row.Field<int>("PaymentID"),
                ApplicationID = row.IsNull("ApplicationID") ? 0 : row.Field<int>("ApplicationID"),
                Amount = row.IsNull("Amount") ? 0 : row.Field<decimal>("Amount"),
                PaymentDate = row.IsNull("PaymentDate") ? DateTime.Now : row.Field<DateTime>("PaymentDate"),
                Status = row.IsNull("Status") ? "Pending" : row.Field<string>("Status"),
                TransactionID = row.IsNull("TransactionID") ? null : row.Field<string>("TransactionID"),
                Application = row.IsNull("ApplicationID") ? null : new Application
                {
                    ApplicationID = row.IsNull("ApplicationID") ? 0 : row.Field<int>("ApplicationID"),
                    StudentID = row.IsNull("StudentID") ? null : row.Field<int>("StudentID"),
                    MajorID = row.IsNull("MajorID") ? null : row.Field<int>("MajorID"),
                    ApplicationDate = row.IsNull("ApplicationDate") ? DateTime.Now : row.Field<DateTime>("ApplicationDate"),
                    Status = row.IsNull("Status") ? "Pending" : row.Field<string>("Status"),
                    Grade = row.IsNull("Grade") ? null : row.Field<decimal>("Grade"),
                    Student = row.IsNull("StudentID") ? null : new Student
                    {
                        StudentID = row.IsNull("StudentID") ? 0 : row.Field<int>("StudentID"),
                        FirstName = row.IsNull("FirstName") ? "Unknown" : row.Field<string>("FirstName"),
                        LastName = row.IsNull("LastName") ? "Student" : row.Field<string>("LastName"),
                        Email = row.IsNull("Email") ? "unknown@student.com" : row.Field<string>("Email"),
                        DateOfBirth = row.IsNull("DateOfBirth") ? DateTime.Now : row.Field<DateTime>("DateOfBirth"),
                        Gender = row.IsNull("Gender") ? "Unknown" : row.Field<string>("Gender"),
                        NationalIDNumber = row.IsNull("NationalIDNumber") ? "Unknown" : row.Field<string>("NationalIDNumber"),
                        SchoolID = row.IsNull("SchoolID") ? null : row.Field<int>("SchoolID"),
                        School = row.IsNull("SchoolName") ? null : new School
                        {
                            SchoolID = row.IsNull("SchoolID") ? 0 : row.Field<int>("SchoolID"),
                            SchoolName = row.IsNull("SchoolName") ? "Unknown" : row.Field<string>("SchoolName"),
                            Email = row.IsNull("Email") ? "unknown@school.com" : row.Field<string>("Email"),
                            Description = row.IsNull("Description") ? "School not found" : row.Field<string>("Description"),
                            TotalScales = row.IsNull("TotalScales") ? null : row.Field<int>("TotalScales"),
                            MinRequiredGrade = row.IsNull("MinRequiredGrade") ? null : row.Field<decimal>("MinRequiredGrade"),
                            EstablishedYear = row.IsNull("EstablishedYear") ? null : row.Field<int>("EstablishedYear"),
                            Majors = new List<Major>(),
                            Students = new List<Student>()
                        },
                        Applications = new List<Application>()
                    },
                    Major = row.IsNull("MajorID") ? null : new Major
                    {
                        MajorID = row.IsNull("MajorID") ? 0 : row.Field<int>("MajorID"),
                        MajorName = row.IsNull("MajorName") ? "Unknown" : row.Field<string>("MajorName"),
                        Description = row.IsNull("Description") ? "No description" : row.Field<string>("Description"),
                        SchoolID = row.IsNull("SchoolID") ? 0 : row.Field<int>("SchoolID"),
                        Capacity = row.IsNull("Capacity") ? null : row.Field<int>("Capacity"),
                        DurationYears = row.IsNull("DurationYears") ? 4 : row.Field<int>("DurationYears"),
                        School = row.IsNull("SchoolName") ? null : new School
                        {
                            SchoolID = row.IsNull("SchoolID") ? 0 : row.Field<int>("SchoolID"),
                            SchoolName = row.IsNull("SchoolName") ? "Unknown" : row.Field<string>("SchoolName"),
                            Email = row.IsNull("Email") ? "unknown@school.com" : row.Field<string>("Email"),
                            Description = row.IsNull("Description") ? "School not found" : row.Field<string>("Description"),
                            TotalScales = row.IsNull("TotalScales") ? null : row.Field<int>("TotalScales"),
                            MinRequiredGrade = row.IsNull("MinRequiredGrade") ? null : row.Field<decimal>("MinRequiredGrade"),
                            EstablishedYear = row.IsNull("EstablishedYear") ? null : row.Field<int>("EstablishedYear"),
                            Majors = new List<Major>(),
                            Students = new List<Student>()
                        },
                        Applications = new List<Application>()
                    }
                }
            });
        }
    }

    public class DatabaseService
    {
        private readonly string? _connectionString;
        private readonly ILogger<DatabaseService> _logger;

        public DatabaseService(IConfiguration configuration, ILogger<DatabaseService> logger)
        {
            _connectionString = configuration.GetConnectionString("UniversityDB");
            _logger = logger;
        }

        public DataTable ExecuteQuery(string query, MySqlParameter[]? parameters = null)
        {
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            _logger.LogInformation("Executing query: {Query}", query);
            DataTable dataTable = new DataTable();

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                using (var command = new MySqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        dataTable.Load(reader);
                    }
                }
                _logger.LogInformation("Query executed successfully");
                return dataTable;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing query: {Query}", query);
                throw;
            }
        }

        public int ExecuteNonQuery(string query, MySqlParameter[]? parameters = null)
        {
            _logger.LogInformation("Executing non-query: {Query}", query);

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                using (var command = new MySqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    connection.Open();
                    var result = command.ExecuteNonQuery();
                    _logger.LogInformation("Non-query executed successfully with result: {Result}", result);
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing non-query: {Query}", query);
                throw;
            }
        }

        public object? ExecuteScalar(string query, MySqlParameter[]? parameters = null)
        {
            _logger.LogInformation("Executing scalar query: {Query}", query);

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                using (var command = new MySqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    connection.Open();
                    var result = command.ExecuteScalar();
                    _logger.LogInformation("Scalar query executed successfully with result: {Result}", result);
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing scalar query: {Query}", query);
                throw;
            }
        }
    }
}