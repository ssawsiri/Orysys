using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using OrysysLoanApplication.Models;
using System.Data;
using System.Reflection;

namespace OrysysLoanApplication.DataAccess
{
    public class DataAccessLoanApplication
    {
        SqlConnection _connection = null;
        SqlCommand _command = null;

        public static IConfiguration Configuration { get; set; }

       
        private string GetConnectionString()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            return Configuration.GetConnectionString("OrysysConnection");
        }

        public bool ValidateUser(string username, string password)
        {
            bool isValidUser = false;
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                using (_command = new SqlCommand("USP_ValidateUserLogin", _connection))
                {
                    _command.CommandType = CommandType.StoredProcedure;
                    _command.Parameters.AddWithValue("@Username", username);
                    _command.Parameters.AddWithValue("@Password", password);
                    _connection.Open();
                    var result = _command.ExecuteScalar();
                    isValidUser = (result != null && Convert.ToInt32(result) == 1);
                    _connection.Close();
                }
            }
        
            return isValidUser;
        }

        public void LogLoginAttempt(string username, bool isSuccess)
        {
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                using (_command = new SqlCommand("USP_InsertLoginAttempt", _connection))
                {
                    _command.CommandType = CommandType.StoredProcedure;
                    _command.Parameters.AddWithValue("@Username", username);
                    _command.Parameters.AddWithValue("@IsSuccess", isSuccess);
                    _connection.Open();
                    _command.ExecuteNonQuery();
                    _connection.Close();
                }
            }
        }

        public List<LoanTypesModel> GetLoanType()
        {
            List<LoanTypesModel> loanTypes = new List<LoanTypesModel>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                using (_command = new SqlCommand("USP_GetLoanTypes", _connection))
                {
                    _command.CommandType = CommandType.StoredProcedure;
                    _connection.Open();
                    using (SqlDataReader reader = _command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            LoanTypesModel loanType = new LoanTypesModel
                            {
                                LoanTypeid = Convert.ToInt32(reader["TypeId"]),
                                LoanTypeName = reader["TypeName"].ToString(),
                                InterestRate = Convert.ToDouble(reader["InterestRate"])
                            };
                            loanTypes.Add(loanType);
                        }
                    }
                    _connection.Close();
                }
            }
            return loanTypes;
        }

        public List<LoanApplicationModel> GetAllLoanApplication()
        {
            List<LoanApplicationModel> loanApplications = new List<LoanApplicationModel>();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                using (_command = new SqlCommand("USP_GetAllLoanApplication", _connection))
                {
                    _command.CommandType = CommandType.StoredProcedure;
                    _connection.Open();
                    using (SqlDataReader reader = _command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            LoanApplicationModel loanapplication = new LoanApplicationModel
                            {
                                LoanID = Convert.ToInt32(reader["LoanId"]),
                                CustomerName = reader["CustomerName"].ToString(),
                                LoanTypeID = Convert.ToInt32(reader["LoanTypeId"]),
                                LoanTypeName = reader["TypeName"].ToString(),
                                InterestRate = Convert.ToDecimal(reader["InterestRate"]),
                                LoanAmount = Convert.ToDecimal(reader["LoanAmount"]),
                                RegisteredDate = Convert.ToDateTime(reader["RegisteredDate"]),
                                Duration = Convert.ToInt32(reader["Duration"]),
                                Status = reader["Status"].ToString(),
                            };
                            loanApplications.Add(loanapplication);
                        }
                    }
                    _connection.Close();
                }
            }
            return loanApplications;
        }
    }
}
