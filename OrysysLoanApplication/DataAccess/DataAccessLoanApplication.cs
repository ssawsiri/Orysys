using Microsoft.Data.SqlClient;
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
    }
}
