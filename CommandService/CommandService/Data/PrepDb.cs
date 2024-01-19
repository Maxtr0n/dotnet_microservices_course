using CommandService.Models;
using CommandService.SyncDataServices.Grpc;

namespace CommandService.Data;
public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder applicationBuilder)
    {
        using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();

        var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
        var commandRepo = serviceScope.ServiceProvider.GetService<ICommandRepo>();

        if (grpcClient == null)
        {
            throw new InvalidOperationException("--> GRPC Client is not available.");
        }

        if (commandRepo == null)
        {
            throw new InvalidOperationException("--> Command repository is not available.");
        }

        var platforms = grpcClient.ReturnAllPlatforms();
        SeedData(commandRepo, platforms);
    }

    private static void SeedData(ICommandRepo commandRepo, IEnumerable<Platform> platforms)
    {
        Console.WriteLine("--> Seeding new platforms...");

        foreach (var platform in platforms)
        {
            if (!commandRepo.ExternalPlatformExists(platform.ExternalId))
            {
                commandRepo.CreatePlatform(platform);
            }
        }

        commandRepo.SaveChanges();
    }
}
