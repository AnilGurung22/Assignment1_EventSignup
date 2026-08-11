using Assignment1_EventSignup.Data;
using Assignment1_EventSignup.Models;
using Assignment1_EventSignup.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Assignment1_EventSignup.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Assignment1_EventSignup.Controllers
{
    [Route("events")]
    public class EventsController : Controller
    {
        private readonly EventManagerContext _context;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IHubContext<EventHub> _hubContext;

        public EventsController(
            EventManagerContext context,
            IBlobStorageService blobStorageService,
            IHubContext<EventHub> hubContext)
        {
            _context = context;
            _blobStorageService = blobStorageService;
            _hubContext = hubContext;
        }

        // GET /events
        [HttpGet("")]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var events = await _context.Events
                .OrderBy(e => e.Date)
                .ToListAsync();
            return View(events);
        }

        // GET /events/{id}
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var ev = await _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (ev == null) return NotFound();
            return View(ev);
        }

        // POST /events/{id}/register  -- authenticated user registers THEMSELVES
        [HttpPost("{id:int}/register")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int id)
        {
            var ev = await _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (ev == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "Unknown";

            // Prevent duplicate registration by the same user
            bool alreadyRegistered = ev.Attendees.Any(a => a.UserId == userId);
            if (alreadyRegistered)
            {
                TempData["Message"] = "You are already registered for this event.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var attendee = new Attendee
            {
                Name = userEmail,
                Email = userEmail,
                EventId = id,
                UserId = userId
            };

            _context.Attendees.Add(attendee);
            await _context.SaveChangesAsync();

            // --- SignalR broadcasts ---

            // 1. Group broadcast: everyone viewing this event gets the new count + attendee name
            int attendeeCount = await _context.Attendees.CountAsync(a => a.EventId == id);
            await _hubContext.Clients.Group($"event-{id}")
                .SendAsync("AttendeeRegistered", userEmail, attendeeCount);

            // 2. Private notification: the Organizer who owns this event
            if (!string.IsNullOrEmpty(ev.OrganizerUserId))
            {
                await _hubContext.Clients.User(ev.OrganizerUserId)
                    .SendAsync("OrganizerNotified",
                        $"{userEmail} just registered for your {ev.Title}.");
            }

            TempData["Message"] = "You have registered for this event.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST /events/{id}/unregister  -- authenticated user removes THEIR OWN registration
        [HttpPost("{id:int}/unregister")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unregister(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var attendee = await _context.Attendees
                .FirstOrDefaultAsync(a => a.EventId == id && a.UserId == userId);

            if (attendee != null)
            {
                _context.Attendees.Remove(attendee);
                await _context.SaveChangesAsync();
                TempData["Message"] = "You have unregistered from this event.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET /events/create
        [HttpGet("create")]
        [Authorize(Roles = "Organizer")]
        public IActionResult Create()
        {
            return View();
        }

        // POST /events/create
        [HttpPost("create")]
        [Authorize(Roles = "Organizer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Title,Description,Date,Location")] Event ev,
            IFormFile? bannerImage)
        {
            if (!ModelState.IsValid) return View(ev);
            if (bannerImage != null && bannerImage.Length > 0)
            {
                ev.BannerUrl = await _blobStorageService.UploadFileAsync(bannerImage);
            }
            ev.OrganizerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _context.Events.Add(ev);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET /events/{id}/edit
        [HttpGet("{id:int}/edit")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Edit(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();
            return View(ev);
        }

        // POST /events/{id}/edit
        [HttpPost("{id:int}/edit")]
        [Authorize(Roles = "Organizer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Title,Description,Date,Location,BannerUrl")] Event ev,
            IFormFile? bannerImage)
        {
            if (id != ev.Id) return NotFound();
            if (!ModelState.IsValid) return View(ev);
            var existing = await _context.Events.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Title = ev.Title;
            existing.Description = ev.Description;
            existing.Date = ev.Date;
            existing.Location = ev.Location;
            if (bannerImage != null && bannerImage.Length > 0)
            {
                existing.BannerUrl = await _blobStorageService.UploadFileAsync(bannerImage);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET /events/{id}/delete
        [HttpGet("{id:int}/delete")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Delete(int id)
        {
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (ev == null) return NotFound();
            return View(ev);
        }

        // POST /events/{id}/delete
        [HttpPost("{id:int}/delete")]
        [Authorize(Roles = "Organizer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev != null)
            {
                _context.Events.Remove(ev);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}