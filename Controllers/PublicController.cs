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

        // Visar alla datorer med status om de är lediga just nu
        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now; // Tidpunkt för tillgänglighetskontroll

            var computers = await _context.Computers
                .Include(c => c.Bookings)
                .ToListAsync();

            var viewModel = computers.Select(c =>
            {
                // Upptagen om någon bokning täcker "now"
                var isBusyNow = c.Bookings.Any(b => b.StartTime <= now && b.EndTime >= now);

                return new ComputerAvailabilityViewModel
                {
                    Id = c.Id,
                    Name = c.Name ?? string.Empty,
                    Location = c.Location ?? string.Empty,
                    IsAvailable = c.IsAvailable,
                    IsAvailableNow = c.IsAvailable && !isBusyNow
                };
            }).ToList();

            return View(viewModel);
        }

        // Visar detaljer för en dator och kommande bokade tider (admin ser användarens e-post)
        public async Task<IActionResult> Details(int id)
        {
            var comp = await _context.Computers
                .Include(c => c.Bookings)
                    .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comp == null) return NotFound();

            var now = DateTime.Now;
            var isAdmin = User.IsInRole("Admin");
            var isBusyNow = comp.Bookings.Any(b => b.StartTime <= now && b.EndTime >= now);

            var vm = new PublicComputerDetailsViewModel
            {
                ComputerId = comp.Id,
                Name = comp.Name,
                Location = comp.Location,
                IsAvailable = comp.IsAvailable,
                IsAvailableNow = comp.IsAvailable && !isBusyNow,
                Slots = comp.Bookings
                    .Where(b => b.EndTime > now)                 // Visa kommande bokningar
                    .OrderBy(b => b.StartTime)
                    .Select(b => new BookingSlotVM
                    {
                        StartTime = b.StartTime,
                        EndTime = b.EndTime,
                        UserEmail = isAdmin ? b.User?.Email : null // Visa e-post endast för admin
                    })
                    .ToList()
            };

            return View(vm);
        }
    }
}
