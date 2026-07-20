using Assignment1_EventSignup.Models;

namespace Assignment1_EventSignup.Data
{
    public static class DbInitializer
    {
        // Ensures the database exists and seeds initial data if empty.
        public static void Initialize(EventManagerContext context)
        {
            context.Database.EnsureCreated();

            // Already seeded
            if (context.Events.Any())
            {
                return;
            }

            var events = new List<Event>
            {
                new Event
                {
                    Title = "Tech Conference 2026",
                    Description = "A full-day conference about cloud, AI, and enterprise apps.",
                    Date = new DateTime(2026, 3, 27, 9, 0, 0),
                    Location = "Ottawa Convention Centre",
                    BannerUrl = null,
                    Attendees = new List<Attendee>
                    {
                        new Attendee { Name = "Alice Smith", Email = "alice@example.com" },
                        new Attendee { Name = "Bob Jones", Email = "bob@example.com" }
                    }
                },
                new Event
                {
                    Title = "Routing Workshop",
                    Description = "Hands-on workshop covering ASP.NET Core attribute routing.",
                    Date = new DateTime(2026, 3, 17, 10, 0, 0),
                    Location = "Algonquin College - T Building",
                    BannerUrl = null,
                    Attendees = new List<Attendee>
                    {
                        new Attendee { Name = "Charlie Brown", Email = "charlie@example.com" },
                        new Attendee { Name = "Dana White", Email = "dana@example.com" }
                    }
                },
                new Event
                {
                    Title = "EF Core Bootcamp",
                    Description = "Deep dive into Entity Framework Core with Azure SQL Database.",
                    Date = new DateTime(2026, 4, 6, 13, 0, 0),
                    Location = "Online",
                    BannerUrl = null,
                    Attendees = new List<Attendee>
                    {
                        new Attendee { Name = "Evan Lee", Email = "evan@example.com" },
                        new Attendee { Name = "Fiona Green", Email = "fiona@example.com" }
                    }
                }
            };

            context.Events.AddRange(events);
            context.SaveChanges();
        }
    }
}
