using Microsoft.AspNetCore.Mvc;
using SimpleWebApp.Models;
using System.Diagnostics;
using System.Reflection;

namespace SimpleWebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IWebHostEnvironment _environment;

    private static readonly List<TimezoneInfo> AvailableTimezones =
    [
        new() { Id = "Central Standard Time", DisplayName = "Central Daylight Time (CDT/CST)" },
        new() { Id = "Eastern Standard Time", DisplayName = "Eastern Daylight Time (EDT/EST)" },
        new() { Id = "Pacific Standard Time", DisplayName = "Pacific Daylight Time (PDT/PST)" },
        new() { Id = "Mountain Standard Time", DisplayName = "Mountain Daylight Time (MDT/MST)" },
        new() { Id = "GMT Standard Time", DisplayName = "Greenwich Mean Time (GMT)" },
        new() { Id = "Israel Standard Time", DisplayName = "Israel Standard Time (IST)" },
        new() { Id = "Tokyo Standard Time", DisplayName = "Japan Standard Time (JST)" },
        new() { Id = "China Standard Time", DisplayName = "China Standard Time (CST)" },
        new() { Id = "India Standard Time", DisplayName = "India Standard Time (IST)" },
        new() { Id = "Central European Standard Time", DisplayName = "Central European Time (CET)" },
        new() { Id = "AUS Eastern Standard Time", DisplayName = "Australian Eastern Standard Time (AEST)" }
    ];

    public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public IActionResult Index()
    {
        // Wall-clock default for datetime-local; UtcNow breaks ConvertTimeToUtc(..., sourceTimeZone) when Kind is Utc.
        var model = new TimezoneConverterViewModel
        {
            AvailableTimezones = [.. AvailableTimezones],
            SourceTime = DateTime.Now
        };
        return View(model);
    }

    [HttpPost]
    public IActionResult ConvertTime(TimezoneConverterViewModel model)
    {
        if (model.SourceTime.HasValue && !string.IsNullOrEmpty(model.SourceTimezone) && !string.IsNullOrEmpty(model.TargetTimezone))
        {
            try
            {
                var sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(model.SourceTimezone);
                var targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(model.TargetTimezone);

                var sourceTimeInUtc = TimeZoneInfo.ConvertTimeToUtc(model.SourceTime.Value, sourceTimeZone);
                model.ConvertedTime = TimeZoneInfo.ConvertTimeFromUtc(sourceTimeInUtc, targetTimeZone);
                model.ConvertedTimeString = model.ConvertedTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (TimeZoneNotFoundException)
            {
                ModelState.AddModelError("", "Invalid timezone selected.");
            }
        }

        model.AvailableTimezones = [.. AvailableTimezones];
        return View("Index", model);
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult InspectMiddleware()
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        try
        {
            var assembly = typeof(human_module.EnforcerConfig).Assembly;
            var types = assembly.GetTypes();
            var middlewareInfo = new System.Text.StringBuilder();

            middlewareInfo.AppendLine($"Assembly: {assembly.FullName}");
            middlewareInfo.AppendLine("\nTypes:");

            foreach (var type in types)
            {
                middlewareInfo.AppendLine($"- {type.FullName}");

                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                var relevantMethods = methods.Where(m => m.Name.Contains("UseHuman") || m.Name.Contains("Middleware"));

                if (relevantMethods.Any())
                {
                    middlewareInfo.AppendLine("  Extension Methods:");
                    foreach (var method in relevantMethods)
                    {
                        middlewareInfo.AppendLine($"    - {method.Name}");
                        middlewareInfo.AppendLine($"      Parameters: {string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"))}");
                    }
                }
            }

            return Content(middlewareInfo.ToString(), "text/plain");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "InspectMiddleware reflection failed.");
            return Content("Unable to inspect middleware.", "text/plain");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
