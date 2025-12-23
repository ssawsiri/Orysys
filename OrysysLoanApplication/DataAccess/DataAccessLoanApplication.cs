using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using OrysysLoanApplication.Controllers;
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
                try
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
                catch (Exception Ex)
                {
                    LogEvents.LogToFile("DataAccessLoanApplication - ValidateUser", Ex.Message);
                }
            }
        
            return isValidUser;
        }

        public void LogLoginAttempt(string username, bool isSuccess)
        {
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                try
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
                catch (Exception Ex)
                {
                    LogEvents.LogToFile("DataAccessLoanApplication - LogLoginAttempt", Ex.Message);
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
                    try
                    {
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
                    }
                    catch (Exception Ex)
                    {
                        LogEvents.LogToFile("DataAccessLoanApplication - GetLoanType", Ex.Message);
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
                    try
                    {
                        using (SqlDataReader reader = _command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                LoanApplicationModel loanapplication = new LoanApplicationModel
                                {
                                    LoanID = Convert.ToInt32(reader["LoanId"]),
                                    CustomerName = reader["CustomerName"].ToString(),
                                    NIC = reader["NIC"].ToString(),
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
                    }
                    catch (Exception Ex)
                    {
                        LogEvents.LogToFile("DataAccessLoanApplication - GetAllLoanApplication", Ex.Message);
                    }
                    _connection.Close();
                }
            }
            return loanApplications;
        }


        public LoanApplicationModel GetLoanApplicationbyId(int loanID)
        {
            LoanApplicationModel loanApplication = new LoanApplicationModel();
            using (_connection = new SqlConnection(GetConnectionString()))
            {
                using (_command = new SqlCommand("USP_GetLoanApplicationById", _connection))
                {
                    _command.CommandType = CommandType.StoredProcedure;
                    _command.Parameters.AddWithValue("@LoanId", loanID);
                    _connection.Open();
                    try
                    {
                        using (SqlDataReader reader = _command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                loanApplication.LoanID = Convert.ToInt32(reader["LoanId"]);
                                loanApplication.CustomerName = reader["CustomerName"].ToString();
                                loanApplication.NIC = reader["NIC"].ToString();
                                loanApplication.LoanTypeID = Convert.ToInt32(reader["LoanTypeId"]);
                                loanApplication.LoanTypeName = reader["TypeName"].ToString();
                                loanApplication.InterestRate = Convert.ToDecimal(reader["InterestRate"]);
                                loanApplication.LoanAmount = Convert.ToDecimal(reader["LoanAmount"]);
                                loanApplication.RegisteredDate = Convert.ToDateTime(reader["RegisteredDate"]);
                                loanApplication.Duration = Convert.ToInt32(reader["Duration"]);
                                loanApplication.Status = reader["Status"].ToString();
                            }
                        }
                    }
                    catch (Exception Ex)
                    {
                        LogEvents.LogToFile("DataAccessLoanApplication - GetLoanApplicationbyId", Ex.Message);
                    }
                    _connection.Close();
                }
            }
            return loanApplication;
        }

        public bool AddLoanapplication (LoanApplicationModel objApplication)
        {
            using (_connection = new SqlConnection(GetConnectionString()))
            using (_command = new SqlCommand("USP_RegisterLoanApplication", _connection))
            {
                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.AddWithValue("@CustomerName", objApplication.CustomerName);
                _command.Parameters.AddWithValue("@NIC", objApplication.NIC);
                _command.Parameters.AddWithValue("@LoanTypeId", objApplication.LoanTypeID);
                _command.Parameters.AddWithValue("@InterestRate", objApplication.InterestRate);
                _command.Parameters.AddWithValue("@LoanAmount", objApplication.LoanAmount);
                _command.Parameters.AddWithValue("@Duration", objApplication.Duration);
                _command.Parameters.AddWithValue("@Status", objApplication.Status);
                try
                {
                    _connection.Open();
                    int rowsAffected = _command.ExecuteNonQuery();
                    _connection.Close();
                    return rowsAffected > 0;
                }
                catch (Exception Ex)
                {
                    LogEvents.LogToFile("DataAccessLoanApplication - AddLoanapplication", Ex.Message);
                    return false;
                }
            }
        }

        public bool UpdateLoanapplication(LoanApplicationModel objApplication)
        {
            using (_connection = new SqlConnection(GetConnectionString()))
            using (_command = new SqlCommand("USP_UpdateLoanApplication", _connection))
            {
                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.AddWithValue("@LoanId", objApplication.LoanID);
                _command.Parameters.AddWithValue("@Status", objApplication.Status);
                
                try
                {
                    _connection.Open();
                    int rowsAffected = _command.ExecuteNonQuery();
                    _connection.Close();
                    return rowsAffected > 0;
                }
                catch (Exception Ex)
                {
                    LogEvents.LogToFile("DataAccessLoanApplication - UpdateLoanapplication", Ex.Message);
                    return false;
                }
            }
        }
    }
}
