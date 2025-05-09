// IP Logger service for collecting and storing user IP addresses
// Made by Shreyas Sama

using AttendanceSystem.Data.Models;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

/// <summary>
/// Service responsible for logging the IP addresses of users when they interact with the system.
/// </summary>
public class IPLoggerService
{
    private readonly AttendanceDbContext _context; // Database context to interact with IP log records
    private readonly IHttpContextAccessor _httpContextAccessor; // Provides access to the current HTTP request context

    /// <summary>
    /// Constructor for IPLoggerService.
    /// </summary>
    /// <param name="context">The database context used to save logs.</param>
    /// <param name="httpContextAccessor">Accessor for HTTP context to retrieve client IP address.</param>
    public IPLoggerService(AttendanceDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Logs the user's IP address to the database.
    /// </summary>
    /// <param name="userId">The ID of the user whose IP is being logged.</param>
    public async Task LogUserIPAsync(int userId)
    {
        // Attempt to retrieve the user's IP address from the current HTTP context
        var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress;

        // Handle the special case for localhost (IPv6 loopback) and null fallback
        string ip = (ipAddress != null && ipAddress.Equals(IPAddress.IPv6Loopback))
            ? "127.0.0.1"
            : ipAddress?.ToString() ?? "Unknown";

        // Create a new IPAddressLog entry
        var log = new IPAddressLog
        {
            UserID = userId,
            IPAddress = ip,
            Timestamp = DateTime.UtcNow // Record the timestamp in UTC
        };

        // Save the log entry to the database
        _context.IPAddressLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
