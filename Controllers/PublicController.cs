using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingSystem.Models;
using BookingSystem.Data;

namespace BookingSystem.Controllers
{
    public class PublicController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PublicController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var computers = await _context.Computers
                .Include(c => c.Bookings)
                .ToListAsync();

            var viewModel = computers.Select(c => new ComputerAvailabilityViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Location = c.Location,
                IsAvailableNow = !c.Bookings.Any(b => b.StartTime <= now && b.EndTime >= now)
            }).ToList();

            return View(viewModel);
        }
    }
}
