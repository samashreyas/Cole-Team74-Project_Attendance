using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using MySql.Data.MySqlClient;
using AttendanceSystem.Models;

/*
        This service handles user authentication and registration for the attendance system.
        It interacts directly with the MySQL database to register users and verify login credentials.
        It also manages user authentication state using a custom AuthenticationStateProvider.
        Made by Jackson Wilson
*/

namespace AttendanceSystem.Services
{
    public class AuthService
    {
        private readonly AuthenticationStateProvider _authProvider;
        private readonly IConfiguration _configuration;

        public AuthService(AuthenticationStateProvider authProvider, IConfiguration configuration)
        {
            _authProvider = authProvider;
            _configuration = configuration;
        }

        // Registers a new user in the MySQL database
        public async Task<bool> RegisterUser(RegistrationModel model)
        {
            try
            {
                // Validate input
                if (model == null || model.UTD_ID <= 0 || string.IsNullOrWhiteSpace(model.Password))
                    throw new ArgumentException("Invalid registration data");

                var connStr = _configuration.GetConnectionString("AttendanceConnection");
                using var conn = new MySqlConnection(connStr);
                await conn.OpenAsync();

                Console.WriteLine($"Registering: UTD_ID={model.UTD_ID}, Role={model.Role}");

                // SQL command to insert user data into the 'user' table
                var cmd = new MySqlCommand("INSERT INTO user (first_name, last_name, UTD_ID, password_hash, role_id) VALUES (@first_name, @last_name, @utd_id, SHA2(@password, 256), @role)", conn);

                // Bind parameters
                cmd.Parameters.Add("@first_name", MySqlDbType.VarChar).Value = model.UTD_ID;
                cmd.Parameters.Add("@last_name", MySqlDbType.VarChar).Value = model.UTD_ID;
                cmd.Parameters.Add("@utd_id", MySqlDbType.Int32).Value = model.UTD_ID;
                cmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = model.Password;
                cmd.Parameters.Add("@role", MySqlDbType.Int32).Value = model.Role;

                // Execute and check success
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
            catch (MySqlException sqlEx)
            {
                Console.WriteLine($" MySQL Registration Error: {sqlEx.Message}");
                throw;
            }
        }

        // Authenticates a user and sets their claims if successful
        public async Task<bool> LoginUser(LoginModel model)
        {
            try
            {
                // Validate input
                if (model == null || model.UTD_ID <= 0 || string.IsNullOrWhiteSpace(model.Password))
                    throw new ArgumentException("Invalid login data");

                var connStr = _configuration.GetConnectionString("AttendanceConnection");
                using var conn = new MySqlConnection(connStr);
                await conn.OpenAsync();

                // SQL query to find the user matching UTD_ID and password hash
                var cmd = new MySqlCommand("SELECT userID, role_id FROM user WHERE UTD_ID = @utd_id AND password_hash = SHA2(@password, 256)", conn);

                // Bind parameters
                cmd.Parameters.Add("@first_name", MySqlDbType.VarChar).Value = model.UTD_ID;
                cmd.Parameters.Add("@last_name", MySqlDbType.VarChar).Value = model.UTD_ID;
                cmd.Parameters.Add("@utd_id", MySqlDbType.Int32).Value = model.UTD_ID;
                cmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = model.Password;

                string roleName = null;
                int userId = -1;

                // Read user info if found
                using var reader = await cmd.ExecuteReaderAsync();
                if (reader.Read())
                {
                    userId = reader.GetInt32(reader.GetOrdinal("userID"));
                    var roleId = reader.GetInt32(reader.GetOrdinal("role_id"));
                    roleName = roleId == 1 ? "Professor" : roleId == 2 ? "Student" : "Unknown";
                }

                // If user is valid, set authentication claims
                if (roleName != null)
                {
                    var identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                        new Claim(ClaimTypes.Role, roleName)
                    }, "apiauth_type");

                    var user = new ClaimsPrincipal(identity);
                    ((CustomAuthenticationStateProvider)_authProvider).SetUser(user);
                    return true;
                }

                Console.WriteLine("Login failed: Invalid credentials or user not found.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Login Error: " + ex.Message);
                return false;
            }
        }

        // Logs the user out by resetting the authentication state
        public void Logout()
        {
            ((CustomAuthenticationStateProvider)_authProvider).SetUser(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }
}
