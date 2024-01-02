using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;
public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();

        SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);
    }

    private static void SeedData(AppDbContext? context, bool isProduction)
    {

        ArgumentNullException.ThrowIfNull(context);

        if (isProduction)
        {
            Console.WriteLine("--> Attempting to apply migrations to database...");
            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not run migrations: {ex.Message}");
            }
        }

        if (!context.Platforms.Any())
        {
            Console.WriteLine("--> Seeding data...");

            context.Platforms.AddRange(
                new Platform() { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
                new Platform() { Name = "Java", Publisher = "JavaCorp", Cost = "200" },
                new Platform() { Name = "Docker", Publisher = "Microsoft", Cost = "50" }
                );

            context.SaveChanges();
        }
        else
        {
            Console.WriteLine("--> We already have data, therefore we do not seed data.");
        }
    }
}