using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using MySql.Data.MySqlClient;

public class LoginService
{
    private readonly string _connectionString;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoginService(string connectionString, IHttpContextAccessor httpContextAccessor)
    {
        _connectionString = connectionString;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<(bool Success, string Message)> AuthenticateUserAsync(string username, string password)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        string query = "SELECT userID, password_hash, role_id FROM user WHERE UTD_ID = @Username";

        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@Username", username);

        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            string hashedPassword = reader["password_hash"].ToString();
            bool isValid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);

            if (isValid)
            {
                int userId = Convert.ToInt32(reader["userID"]);
                int roleId = Convert.ToInt32(reader["role_id"]);

                //  Set up the claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()), // required
                    new Claim(ClaimTypes.Role, roleId.ToString())             // optional
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                //  Sign in the user
                await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return (true, "Login successful.");
            }

            return (false, "Incorrect password.");
        }

        return (false, "User not found.");
    }
}
