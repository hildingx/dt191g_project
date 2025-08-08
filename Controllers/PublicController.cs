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

        public async Task<IActionResult> Details(int id)
        {
            var comp = await _context.Computers
                .Include(c => c.Bookings)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comp == null) return NotFound();

            var now = DateTime.Now;
            var vm = new PublicComputerDetailsViewModel
            {
                ComputerId = comp.Id,
                Name = comp.Name,
                Location = comp.Location,
                IsAvailableNow = !comp.Bookings.Any(b => b.StartTime <= now && b.EndTime >= now),
                Slots = comp.Bookings
                    .Where(b => b.EndTime > now)
                    .OrderBy(b => b.StartTime)
                    .Select(b => new BookingSlotVM { StartTime = b.StartTime, EndTime = b.EndTime })
                    .ToList()
            };

            return View(vm);
        }
    }
}
