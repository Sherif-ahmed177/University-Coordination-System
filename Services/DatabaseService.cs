using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using UniversityApplicationSystem.Models;

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
                NationalID = row.Field<string>("NationalID"),
                SchoolID = row.Field<int?>("SchoolID"),
                School = new School
                {
                    SchoolID = row.Field<int?>("SchoolID") ?? 0,
                    SchoolName = "Unknown",
                    Email = "unknown@school.com",
                    Description = "School not found",
                    Majors = new List<Major>(),
                    Students = new List<Student>()
                },
                Applications = new List<Application>()
            });
        }

        public static IEnumerable<Major> ToMajors(this DataTable table)
        {
            return table.AsEnumerable().Select(row => new Major
            {
                MajorID = row.Field<int>("MajorID"),
                Name = row.Field<string>("Name"),
                Description = row.Field<string>("Description"),
                SchoolID = row.Field<int>("SchoolID"),
                Capacity = row.Field<int?>("Capacity"),
                DurationYears = row.Field<int>("DurationYears"),
                School = new School
                {
                    SchoolID = row.Field<int>("SchoolID"),
                    SchoolName = "Unknown",
                    Email = "unknown@school.com",
                    Description = "School not found",
                    Majors = new List<Major>(),
                    Students = new List<Student>()
                },
                Applications = new List<Application>()
            });
        }

        public static IEnumerable<Application> ToApplications(this DataTable table)
        {
            return table.AsEnumerable().Select(row => new Application
            {
                ApplicationID = row.Field<int>("ApplicationID"),
                StudentID = row.Field<int>("StudentID"),
                MajorID = row.Field<int>("MajorID"),
                ApplicationDate = row.Field<DateTime>("ApplicationDate"),
                Status = row.Field<string>("Status"),
                Grade = row.Field<decimal?>("Grade"),
                Notes = row.Table.Columns.Contains("Notes") ? row.Field<string>("Notes") : null,
                Student = new Student
                {
                    StudentID = row.Field<int>("StudentID"),
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
                },
                Major = new Major
                {
                    MajorID = row.Field<int>("MajorID"),
                    Name = row.Field<string>("Name") ?? "Unknown",
                    Description = row.Field<string>("Description") ?? "Major not found",
                    SchoolID = row.Field<int>("SchoolID"),
                    School = new School
                    {
                        SchoolID = row.Field<int>("SchoolID"),
                        SchoolName = "Unknown",
                        Email = "unknown@school.com",
                        Description = "School not found",
                        Majors = new List<Major>(),
                        Students = new List<Student>()
                    },
                    Applications = new List<Application>()
                },
                Payment = new Payment
                {
                    ApplicationID = row.Field<int>("ApplicationID"),
                    Status = "Pending",
                    Amount = 0,
                    PaymentDate = DateTime.Now,
                    Application = null! // Will be set by the caller
                }
            });
        }

        public static IEnumerable<Payment> ToPayments(this DataTable table)
        {
            return table.AsEnumerable().Select(row => new Payment
            {
                PaymentID = row.Field<int>("PaymentID"),
                ApplicationID = row.Field<int>("ApplicationID"),
                Amount = row.Field<decimal>("Amount"),
                PaymentDate = row.Field<DateTime>("PaymentDate"),
                Status = row.Field<string>("Status"),
                TransactionID = row.Field<string>("TransactionID"),
                Application = new Application
                {
                    ApplicationID = row.Field<int>("ApplicationID"),
                    StudentID = 0,
                    MajorID = 0,
                    ApplicationDate = DateTime.Now,
                    Status = "Unknown",
                    Student = new Student
                    {
                        StudentID = 0,
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
                    },
                    Major = new Major
                    {
                        MajorID = 0,
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
                    },
                    Payment = null! // Will be set by the caller
                }
            });
        }
    }

    public class DatabaseService
    {
        private readonly string? _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("UniversityDB");
        }

        public DataTable ExecuteQuery(string query, MySqlParameter[]? parameters = null)
        {
            DataTable dataTable = new DataTable();

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

            return dataTable;
        }

        public int ExecuteNonQuery(string query, MySqlParameter[]? parameters = null)
        {
            using (var connection = new MySqlConnection(_connectionString))
            using (var command = new MySqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public object? ExecuteScalar(string query, MySqlParameter[]? parameters = null)
        {
            using (var connection = new MySqlConnection(_connectionString))
            using (var command = new MySqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                connection.Open();
                return command.ExecuteScalar();
            }
        }
    }
}