using Assignment1_EventSignup.Data;
using Assignment1_EventSignup.Models;
using Assignment1_EventSignup.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment1_EventSignup.Controllers
{
    [Route("events")]
    public class EventsController : Controller
    {
        private readonly EventManagerContext _context;
        private readonly IBlobStorageService _blobStorageService;

        public EventsController(EventManagerContext context, IBlobStorageService blobStorageService)
        {
            _context = context;
            _blobStorageService = blobStorageService;
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