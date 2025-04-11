namespace AttendanceSystem.Services
{
    public interface IUserService
    {
        Task<UserDto> AuthenticateAsync(string utdId, string password);
    }

    public class UserDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RoleName { get; set; }
    }
}