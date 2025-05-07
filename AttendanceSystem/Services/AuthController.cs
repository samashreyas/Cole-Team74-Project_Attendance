using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AttendanceSystem.Services;

namespace AttendanceSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly LoginService _loginService;

        public AuthController(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            string connStr = config.GetConnectionString("DefaultConnection");
            _loginService = new LoginService(connStr, httpContextAccessor);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var (success, message) = await _loginService.AuthenticateUserAsync(request.Username, request.Password);

            if (success)
                return Ok(new { success = true });
            else
                return Unauthorized(new { success = false, message });
        }

        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}