using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AttendanceSystem.Data;
using AttendanceSystem.Areas.Identity;
using AttendanceSystem.Data.Models;  // Add this for AttendanceDbContext
using AttendanceSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Original SQL Server connection for Identity
var identityConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Add MySQL connection for Attendance System
var attendanceConnectionString = builder.Configuration.GetConnectionString("AttendanceConnection") ??
    throw new InvalidOperationException("Connection string 'AttendanceConnection' not found.");

builder.Services.AddDbContext<AttendanceDbContext>(options =>
    options.UseMySql(attendanceConnectionString, ServerVersion.AutoDetect(attendanceConnectionString)));

// Registering other services for Identity and Authentication
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
// Register services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<DatabaseService>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddAuthorizationCore();
builder.Services.AddSingleton<WeatherForecastService>();

// Services for IP access and logging
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPLoggerService>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
