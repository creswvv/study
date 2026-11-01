using CollegeManagement.Data;
using CollegeManagement.Models;
using CollegeManagement.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// SQLite
builder.Services.AddDbContext<CollegeDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=college.db"));

// Авторизация через cookie
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Account/Login";
        o.AccessDeniedPath = "/Account/Denied";
        o.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

// internal-класс CollegeSettings регистрируется через публичный интерфейс ICollegeInfo
builder.Services.AddSingleton<ICollegeInfo>(new CollegeSettings(
    builder.Configuration["College:Name"] ?? "Колледж",
    builder.Configuration["College:AcademicYear"] ?? "2026/2027"));

builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<ReportManager>();
builder.Services.AddScoped<AttendanceManager>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CollegeDbContext>();
    DbSeeder.Seed(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
