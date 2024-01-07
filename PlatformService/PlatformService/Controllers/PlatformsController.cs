using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepo _platformRepo;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _commandDataClient;
    private readonly IMessageBusClient _messageBusClient;

    public PlatformsController(
        IPlatformRepo platformRepo,
        IMapper mapper,
        ICommandDataClient commandDataClient,
        IMessageBusClient messageBusClient)
    {
        _platformRepo = platformRepo;
        _mapper = mapper;
        _commandDataClient = commandDataClient;
        _messageBusClient = messageBusClient;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        Console.WriteLine("--> Getting Platforms...");

        var platforms = _platformRepo.GetAllPlatforms();

        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
    }

    [HttpGet("{id}", Name = "GetPlatformById")]
    public ActionResult<PlatformReadDto> GetPlatformById(int id)
    {
        Console.WriteLine("--> Getting Platform By Id...");

        var platform = _platformRepo.GetPlatformById(id);

        if (platform == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<PlatformReadDto>(platform));
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformToCreate)
    {
        Console.WriteLine("--> Creating Platform...");

        var platform = _mapper.Map<Platform>(platformToCreate);
        _platformRepo.CreatePlatform(platform);
        _platformRepo.SaveChanges();

        var platformReadDto = _mapper.Map<PlatformReadDto>(platform);

        // Send Sync Message
        try
        {
            await _commandDataClient.SendPlatformToCommand(platformReadDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
        }

        // Send Async Message
        try
        {
            var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
            platformPublishedDto.Event = "Platform_Published";

            _messageBusClient.PublishNewPlatform(platformPublishedDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
        }

        return CreatedAtRoute(nameof(GetPlatformById), new { platformReadDto.Id }, platformReadDto);
    }
}