using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using BookingSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

    public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;

            var featured = await _context.Computers
                .Include(c => c.Bookings)
                .Select(c => new ComputerAvailabilityViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Location = c.Location,
                    IsAvailableNow = !c.Bookings.Any(b => b.StartTime <= now && b.EndTime >= now)
                })
                .Where(c => c.IsAvailableNow)
                .OrderBy(c => c.Name)
                .Take(3)
                .ToListAsync();

            var vm = new HomeIndexViewModel
            {
                FeaturedComputers = featured
            };

            return View(vm);
        }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
