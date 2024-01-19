using AutoMapper;
using CommandService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandService.SyncDataServices.Grpc;
public class PlatformDataClient(IConfiguration configuration, IMapper mapper) : IPlatformDataClient
{
    public IEnumerable<Platform> ReturnAllPlatforms()
    {
        var grpcPlatformConfig = configuration["GrpcPlatform"];

        if (grpcPlatformConfig == null)
        {
            throw new ArgumentNullException(nameof(grpcPlatformConfig));
        }

        Console.WriteLine($"--> Calling GRPC Service {grpcPlatformConfig}");
        var channel = GrpcChannel.ForAddress(grpcPlatformConfig);
        var client = new GrpcPlatform.GrpcPlatformClient(channel);
        var request = new GetAllRequest();

        try
        {
            var reply = client.GetAllPlatforms(request);
            return mapper.Map<IEnumerable<Platform>>(reply.Platform);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not call GRPC Server: {ex.Message}");
            return [];
        }
    }
}
