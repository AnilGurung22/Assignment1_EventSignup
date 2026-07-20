using Assignment1_EventSignup.Data;
using Assignment1_EventSignup.Services;
using Microsoft.EntityFrameworkCore;

namespace Assignment1_EventSignup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // EF Core / Azure SQL Database
            builder.Services.AddDbContext<EventManagerContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Azure Blob Storage
            builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

            var app = builder.Build();

            // Ensure database exists and is seeded.
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EventManagerContext>();
                DbInitializer.Initialize(context);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Events}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
