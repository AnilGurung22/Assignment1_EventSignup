using Assignment1_EventSignup.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment1_EventSignup.Controllers
{
    public class EventController : Controller
    {
        // static keeps data alive between requests (simulates a database)
        private static List<Event> _events = new List<Event>
{
    new Event { Id = 1, Title = "AI Workshop",      Date = new DateTime(2026, 3, 5),  Location = "Room 204" },
    new Event { Id = 2, Title = "Networking Night", Date = new DateTime(2026, 3, 12), Location = "Main Hall" },
    new Event { Id = 3, Title = "Code Sprint",      Date = new DateTime(2026, 3, 19), Location = "Lab 310" }
};

        // GET: /Event/Index
        public IActionResult Index()
        {
            return View(_events);
        }

        // GET: /Event/ManageAttendees/1
        public IActionResult ManageAttendees(int id)
        {
            var ev = _events.FirstOrDefault(e => e.Id == id);
            if (ev == null) return NotFound();

            ViewData["EventName"] = ev.Title;
            return View(ev);
        }

        // POST: /Event/SignUp
        [HttpPost]
        public IActionResult SignUp(int eventId, string name, string email)
        {
            var ev = _events.FirstOrDefault(e => e.Id == eventId);
            if (ev != null && !string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(email))
            {
                ev.Attendees.Add(new Attendee { Name = name, Email = email });
                ViewData["Message"] = "Attendee registered!";
            }

            ViewData["EventName"] = ev?.Title;
            return View("ManageAttendees", ev);
        }
    }
}