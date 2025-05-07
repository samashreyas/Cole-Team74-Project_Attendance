using AttendanceSystem.Data.Models;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class IPLoggerService
{
    private readonly AttendanceDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IPLoggerService(AttendanceDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogUserIPAsync(int userId)
    {
        var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress;
        string ip = (ipAddress != null && ipAddress.Equals(IPAddress.IPv6Loopback))
            ? "127.0.0.1"
            : ipAddress?.ToString() ?? "Unknown";

        var log = new IPAddressLog
        {
            UserID = userId,
            IPAddress = ip,
            Timestamp = DateTime.UtcNow
        };

        _context.IPAddressLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}