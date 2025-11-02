using PrintingJobTracker.Infrastructure.Seed;
using PrintingJobTracker.Application;
using PrintingJobTracker.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using PrintingJobTracker.Data;
using PrintingJobTracker.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddApplication();
builder.Services.AddServerSideBlazor();

// DI for seeders (if present)
builder.Services.AddScoped<PrintingJobTracker.Services.DataSeeder>();
builder.Services.AddScoped<PrintingJobTracker.Infrastructure.Seed.DataInitializer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Migrate & seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<PrintingJobTracker.Data.OpsDbContext>();
        await db.Database.MigrateAsync();

        var seeder = services.GetService<PrintingJobTracker.Services.DataSeeder>();
        if (seeder is not null)
        {
            await seeder.SeedAsync();
        }

        var initializer = services.GetService<PrintingJobTracker.Infrastructure.Seed.DataInitializer>();
        if (initializer is not null)
        {
            await initializer.SeedAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

app.Run();
