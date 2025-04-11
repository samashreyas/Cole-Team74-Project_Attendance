using Microsoft.Extensions.Configuration;
using MySqlConnector; // Note: Using MySqlConnector instead of MySql.Data
using Dapper;
using System.Threading.Tasks;

namespace AttendanceSystem.Services
{
    public class UserService : IUserService
    {
        private readonly string _connectionString;
        
        public UserService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AttendanceConnection");
        }
        
        public async Task<UserDto> AuthenticateAsync(string utdId, string password)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var query = @"SELECT u.userID, u.first_name, u.last_name, r.role_name 
                        FROM user u 
                        JOIN roles r ON u.role_id = r.roleID
                        WHERE u.UTD_ID = @UtdId AND u.password_hash = @PasswordHash";
            
            var user = await connection.QueryFirstOrDefaultAsync<UserDto>(query, 
                new { UtdId = utdId, PasswordHash = password });
                
            return user;
        }
    }
}