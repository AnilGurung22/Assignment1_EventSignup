using Assignment1_EventSignup.Data;
using Assignment1_EventSignup.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment1_EventSignup.Controllers
{
    // Attendees are always managed within the context of an Event.
    [Route("events/{eventId:int}/attendees")]
    public class AttendeesController : Controller
    {
        private readonly EventManagerContext _context;

        public AttendeesController(EventManagerContext context)
        {
            _context = context;
        }

        private async Task<Event?> GetEventAsync(int eventId)
        {
            return await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);
        }

        // GET /events/{eventId}/attendees
        [HttpGet("")]
        public async Task<IActionResult> Index(int eventId)
        {
            var ev = await _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (ev == null) return NotFound();

            ViewData["EventId"] = ev.Id;
            ViewData["EventName"] = ev.Title;
            return View(ev.Attendees);
        }

        // GET /events/{eventId}/attendees/create
        [HttpGet("create")]
        public async Task<IActionResult> Create(int eventId)
        {
            var ev = await GetEventAsync(eventId);
            if (ev == null) return NotFound();

            ViewData["EventId"] = ev.Id;
            ViewData["EventName"] = ev.Title;
            return View();
        }

        // POST /events/{eventId}/attendees/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int eventId, [Bind("Name,Email")] Attendee attendee)
        {
            var ev = await GetEventAsync(eventId);
            if (ev == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["EventId"] = ev.Id;
                ViewData["EventName"] = ev.Title;
                return View(attendee);
            }

            attendee.EventId = eventId;
            _context.Attendees.Add(attendee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { eventId });
        }

        // GET /events/{eventId}/attendees/{id}/edit
        [HttpGet("{id}/edit")]
        public async Task<IActionResult> Edit(int eventId, string id)
        {
            var ev = await GetEventAsync(eventId);
            if (ev == null) return NotFound();

            var attendee = await _context.Attendees
                .FirstOrDefaultAsync(a => a.Id == id && a.EventId == eventId);
            if (attendee == null) return NotFound();

            ViewData["EventId"] = ev.Id;
            ViewData["EventName"] = ev.Title;
            return View(attendee);
        }

        // POST /events/{eventId}/attendees/{id}/edit
        [HttpPost("{id}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int eventId, string id, [Bind("Id,Name,Email,EventId")] Attendee attendee)
        {
            if (id != attendee.Id || eventId != attendee.EventId) return NotFound();

            var ev = await GetEventAsync(eventId);
            if (ev == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["EventId"] = ev.Id;
                ViewData["EventName"] = ev.Title;
                return View(attendee);
            }

            var existing = await _context.Attendees
                .FirstOrDefaultAsync(a => a.Id == id && a.EventId == eventId);
            if (existing == null) return NotFound();

            existing.Name = attendee.Name;
            existing.Email = attendee.Email;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { eventId });
        }

        // GET /events/{eventId}/attendees/{id}/delete
        [HttpGet("{id}/delete")]
        public async Task<IActionResult> Delete(int eventId, string id)
        {
            var ev = await GetEventAsync(eventId);
            if (ev == null) return NotFound();

            var attendee = await _context.Attendees
                .FirstOrDefaultAsync(a => a.Id == id && a.EventId == eventId);
            if (attendee == null) return NotFound();

            ViewData["EventId"] = ev.Id;
            ViewData["EventName"] = ev.Title;
            return View(attendee);
        }

        // POST /events/{eventId}/attendees/{id}/delete
        [HttpPost("{id}/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int eventId, string id)
        {
            var attendee = await _context.Attendees
                .FirstOrDefaultAsync(a => a.Id == id && a.EventId == eventId);
            if (attendee != null)
            {
                _context.Attendees.Remove(attendee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { eventId });
        }
    }
}
