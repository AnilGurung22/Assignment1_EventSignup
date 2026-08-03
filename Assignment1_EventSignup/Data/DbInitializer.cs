using Assignment1_EventSignup.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Assignment1_EventSignup.Data
{
    public static class DbInitializer
    {
        // Ensures the database exists and seeds initial event data if empty.
        public static void Initialize(EventManagerContext context)
        {
            context.Database.Migrate();

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

        // Seeds Organizer/Attendee roles and two users. Idempotent - safe to run every startup.
        public static async Task SeedRolesAndUsers(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = { "Organizer", "Attendee" };

            // Create roles if they don't exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed an Organizer user
            await CreateUser(userManager, "organizer@example.com", "Organizer123!", "Organizer");

            // Seed an Attendee user
            await CreateUser(userManager, "attendee@example.com", "Attendee123!", "Attendee");
        }

        private static async Task CreateUser(
            UserManager<IdentityUser> userManager,
            string email,
            string password,
            string role)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}