using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookingSystem.Data;
using Microsoft.AspNetCore.Authorization;

namespace BookingSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ComputerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ComputerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Visar lista över alla datorer
        public async Task<IActionResult> Index()
        {
            return View(await _context.Computers.ToListAsync());
        }

        // Visar detaljer för en specifik dator
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var computer = await _context.Computers
                .FirstOrDefaultAsync(m => m.Id == id);

            if (computer == null) return NotFound();

            return View(computer);
        }

        // Visar formulär för att skapa en ny dator
        public IActionResult Create()
        {
            return View();
        }

        // Skapar en ny dator
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Location,IsAvailable")] Computer computer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(computer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(computer);
        }

        // Visar formulär för att redigera en dator
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var computer = await _context.Computers.FindAsync(id);
            if (computer == null) return NotFound();

            return View(computer);
        }

        // Uppdaterar en dator
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Location,IsAvailable")] Computer computer)
        {
            if (id != computer.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(computer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComputerExists(computer.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(computer);
        }

        // Visar bekräftelsesida för borttagning av en dator
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var computer = await _context.Computers
                .FirstOrDefaultAsync(m => m.Id == id);

            if (computer == null) return NotFound();

            return View(computer);
        }

        // Tar bort dator efter bekräftelse
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var computer = await _context.Computers.FindAsync(id);
            if (computer != null)
            {
                _context.Computers.Remove(computer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Kontroll om dator finns
        private bool ComputerExists(int id)
        {
            return _context.Computers.Any(e => e.Id == id);
        }
    }
}
