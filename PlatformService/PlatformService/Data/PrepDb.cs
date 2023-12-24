using PlatformService.Models;

namespace PlatformService.Data;
public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app)
    {
        using IServiceScope serviceScope = app.ApplicationServices.CreateScope();

        SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
    }

    private static void SeedData(AppDbContext? context)
    {
        ArgumentNullException.ThrowIfNull(context);

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