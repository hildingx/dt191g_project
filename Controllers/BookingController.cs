using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookingSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BookingSystem.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BookingController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Tillåter ägare av bokningen eller admins
        private bool IsOwnerOrAdmin(Booking b)
            => b.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin");

        // Visar kommande bokningar för inloggad användare
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            // Filtrera på inloggad användare och endast framtida bokningar
            var bookings = _context.Bookings
                .Include(b => b.Computer)
                .Where(b => b.UserId == userId && b.EndTime > DateTime.Now);

            return View(await bookings.ToListAsync());
        }

        // Visar detaljer för en specifik bokning
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Computer)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // Visar formulär för att skapa bokning med endast bokningsbara datorer
        public IActionResult Create(int? computerId)
        {
            var selectable = _context.Computers
                .Where(c => c.IsAvailable)
                .ToList();

            // Förifyll valbar lista, ev. förvald dator
            ViewData["ComputerId"] = new SelectList(selectable, "Id", "Name", computerId);
            return View();
        }

        // Skapar bokning efter validering (överlapp, tider, maxlängd, datorns status)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ComputerId,StartTime,EndTime")] Booking booking)
        {
            var now = DateTime.Now;

            // Sätt ägare till inloggad användare och ta bort ev. bindningsfel
            booking.UserId = _userManager.GetUserId(User);
            ModelState.Remove(nameof(booking.UserId));

            // Kontrollera att vald dator finns
            var computer = await _context.Computers.FindAsync(booking.ComputerId);
            if (computer == null) return NotFound();

            // Datorn måste vara aktiv/bokningsbar
            if (!computer.IsAvailable)
            {
                ModelState.AddModelError("", "Datorn är avstängd och kan inte bokas.");
            }

            // Överlappande bokning på samma dator
            bool isOverlapping = _context.Bookings.Any(b =>
                b.ComputerId == booking.ComputerId &&
                b.EndTime > booking.StartTime &&
                b.StartTime < booking.EndTime);

            if (isOverlapping)
            {
                ModelState.AddModelError("", "Datorn är redan bokad under valt tidsintervall.");
            }

            // Starttid får inte ligga i det förflutna
            if (booking.StartTime < now)
            {
                ModelState.AddModelError(nameof(booking.StartTime), "Du kan inte boka en tid som passerat.");
            }

            // Sluttid måste vara efter starttid
            if (booking.EndTime <= booking.StartTime)
            {
                ModelState.AddModelError(nameof(booking.EndTime), "Sluttiden måste vara efter starttiden.");
            }

            // Maxlängd 2 timmar
            var maxBookingLength = TimeSpan.FromHours(2);
            if (booking.EndTime - booking.StartTime > maxBookingLength)
            {
                ModelState.AddModelError("", "En bokning får vara max 2 timmar.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Återfyll valbar lista vid valideringsfel
            var selectable = _context.Computers.Where(c => c.IsAvailable).ToList();
            ViewData["ComputerId"] = new SelectList(selectable, "Id", "Name", booking.ComputerId);
            return View(booking);
        }

        // Visar formulär för att redigera bokning, endast tillåtet för ägare/admin
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Computer)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null) return NotFound();

            // Behörighetskontroll
            if (booking.UserId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
                return Forbid();

            // Lista aktiva datorer för val
            var active = await _context.Computers
                .Where(c => c.IsAvailable)
                .AsNoTracking()
                .ToListAsync();

            // Om nuvarande dator är avstängd, visa den överst + varning
            if (booking.Computer is { IsAvailable: false })
            {
                active.Insert(0, booking.Computer);
                ViewData["ComputerWarning"] = "Nuvarande dator är avstängd. Välj en annan innan du sparar.";
                ViewData["ComputerSelectList"] = active
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.IsAvailable ? c.Name : $"{c.Name} (Avstängd)",
                        Selected = c.Id == booking.ComputerId
                    }).ToList();
            }
            else
            {
                ViewData["ComputerSelectList"] = new SelectList(active, "Id", "Name", booking.ComputerId);
            }

            return View(booking);
        }

        // Uppdaterar bokning efter validering och behörighetskontroll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ComputerId,StartTime,EndTime,UserId")] Booking booking)
        {
            if (id != booking.Id) return NotFound();

            // Behörighetskontroll
            if (!IsOwnerOrAdmin(booking)) return Forbid();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // Återfyll listor vid valideringsfel
            ViewData["ComputerId"] = new SelectList(_context.Computers, "Id", "Name", booking.ComputerId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", booking.UserId);
            return View(booking);
        }

        // Hämtar en specifik bokning och visar bekräftelsesidan för borttagning. Endast ägare eller admin får ta bort.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Computer)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null) return NotFound();

            // Behörighetskontroll
            if (!IsOwnerOrAdmin(booking)) return Forbid();

            return View(booking);
        }

        // Tar bort bokning efter bekräftelse. Endast ägare eller admin får ta bort.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            // Behörighetskontroll
            if (!IsOwnerOrAdmin(booking!)) return Forbid();

            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Kontroll om bokning finns
        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }
    }
}
