using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace YourNamespace.Services
{
    public class LoginService
    {
        private readonly string _connectionString;

        public LoginService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<(bool Success, string Message)> AuthenticateUserAsync(string username, string password)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = "SELECT password_hash FROM user WHERE UTD_ID = @Username";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Username", username);

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                string hashedPassword = reader["password_hash"].ToString();
                bool isValid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);

                if (isValid)
                    return (true, "Login successful.");
                else
                    return (false, "Incorrect password.");
            }

            return (false, "User not found.");
        }
    }
}
