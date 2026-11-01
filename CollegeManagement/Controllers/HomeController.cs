using CollegeManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagement.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ReportManager _reports;
    public HomeController(ReportManager reports) => _reports = reports;

    public IActionResult Index() => View(_reports.GetDashboard());

    [AllowAnonymous]
    public IActionResult Error() => Content("Произошла ошибка. Вернитесь на главную страницу.");
}
