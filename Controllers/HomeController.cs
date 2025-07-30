using Microsoft.AspNetCore.Mvc;
using SimpleWebApp.Models;
using System.Diagnostics;
using System.Reflection;

namespace SimpleWebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var model = new TimezoneConverterViewModel
        {
            AvailableTimezones = GetAvailableTimezones(),
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
        
        model.AvailableTimezones = GetAvailableTimezones();
        return View("Index", model);
    }

    public IActionResult About()
    {
        return View();
    }

    private List<TimezoneInfo> GetAvailableTimezones()
    {
        var commonTimezones = new Dictionary<string, string>
        {
            { "Central Standard Time", "Central Daylight Time (CDT/CST)" },
            { "Eastern Standard Time", "Eastern Daylight Time (EDT/EST)" },
            { "Pacific Standard Time", "Pacific Daylight Time (PDT/PST)" },
            { "Mountain Standard Time", "Mountain Daylight Time (MDT/MST)" },
            { "GMT Standard Time", "Greenwich Mean Time (GMT)" },
            { "Israel Standard Time", "Israel Standard Time (IST)" },
            { "Tokyo Standard Time", "Japan Standard Time (JST)" },
            { "China Standard Time", "China Standard Time (CST)" },
            { "India Standard Time", "India Standard Time (IST)" },
            { "Central European Standard Time", "Central European Time (CET)" },
            { "AUS Eastern Standard Time", "Australian Eastern Standard Time (AEST)" }
        };

        return commonTimezones.Select(tz => new TimezoneInfo
        {
            Id = tz.Key,
            DisplayName = tz.Value
        }).ToList();
    }

    public IActionResult InspectMiddleware()
    {
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
            return Content($"Error: {ex.Message}\n\nStack: {ex.StackTrace}", "text/plain");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}