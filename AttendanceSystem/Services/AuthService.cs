using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using MySql.Data.MySqlClient;
using AttendanceSystem.Models;

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

        public async Task<bool> RegisterUser(RegistrationModel model)
        {
            try
            {
                if (model == null || model.UTD_ID <= 0 || string.IsNullOrWhiteSpace(model.Password))
                    throw new ArgumentException("Invalid registration data");

                var connStr = _configuration.GetConnectionString("AttendanceConnection");
                using var conn = new MySqlConnection(connStr);
                await conn.OpenAsync();

                Console.WriteLine($"Registering: UTD_ID={model.UTD_ID}, Role={model.Role}");

                var cmd = new MySqlCommand("INSERT INTO user (first_name, last_name, UTD_ID, password_hash, role_id) VALUES (@first_name, @last_name, @utd_id, SHA2(@password, 256), @role)", conn);
                cmd.Parameters.Add("@first_name", MySqlDbType.VarChar).Value = model.UTD_ID;
                cmd.Parameters.Add("@last_name", MySqlDbType.VarChar).Value = model.UTD_ID;
                cmd.Parameters.Add("@utd_id", MySqlDbType.Int32).Value = model.UTD_ID;
                cmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = model.Password;
                cmd.Parameters.Add("@role", MySqlDbType.Int32).Value = model.Role;

                return await cmd.ExecuteNonQueryAsync() > 0;
            }
            catch (MySqlException sqlEx)
            {
                Console.WriteLine($" MySQL Registration Error: {sqlEx.Message}");
                throw;
            }
        }

        public async Task<bool> LoginUser(LoginModel model)
        {
            try
            {
                if (model == null || model.UTD_ID <= 0 || string.IsNullOrWhiteSpace(model.Password))
                    throw new ArgumentException("Invalid login data");

                var connStr = _configuration.GetConnectionString("AttendanceConnection");
                using var conn = new MySqlConnection(connStr);
                await conn.OpenAsync();

                var cmd = new MySqlCommand("SELECT userID, role_id FROM user WHERE UTD_ID = @utd_id AND password_hash = SHA2(@password, 256)", conn);
                cmd.Parameters.Add("@first_name", MySqlDbType.VarChar).Value = model.UTD_ID;
                cmd.Parameters.Add("@last_name", MySqlDbType.VarChar).Value = model.UTD_ID;
                cmd.Parameters.Add("@utd_id", MySqlDbType.Int32).Value = model.UTD_ID;
                cmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = model.Password;

                string roleName = null;
                int userId = -1;

                using var reader = await cmd.ExecuteReaderAsync();
                if (reader.Read())
                {
                    userId = reader.GetInt32(reader.GetOrdinal("userID"));
                    var roleId = reader.GetInt32(reader.GetOrdinal("role_id"));
                    roleName = roleId == 1 ? "Professor" : roleId == 2 ? "Student" : "Unknown";
                }

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

        public void Logout()
        {
            ((CustomAuthenticationStateProvider)_authProvider).SetUser(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }
}