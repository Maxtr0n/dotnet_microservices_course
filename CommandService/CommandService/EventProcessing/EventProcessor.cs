﻿using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using System.Text.Json;

namespace CommandService.EventProcessing;
public class EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper) : IEventProcessor
{
    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch (eventType)
        {
            case EventType.PlatformPublished:
                AddPlatform(message);
                break;
            default:
                break;
        }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
        Console.WriteLine("--> Determining Event");

        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

        if (eventType == null)
        {
            return EventType.Undetermined;
        }

        switch (eventType.Event)
        {
            case "Platform_Published":
                Console.WriteLine("--> Platform Published event detected");
                return EventType.PlatformPublished;
            default:
                Console.WriteLine($"--> Could not determine the event type.");
                return EventType.Undetermined;
        }
    }

    private void AddPlatform(string platformPublishedMessage)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

        var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

        try
        {
            var plat = mapper.Map<Platform>(platformPublishedDto);
            if (!repo.ExternalPlatformExists(plat.ExternalId))
            {
                repo.CreatePlatform(plat);
                repo.SaveChanges();
                Console.WriteLine("--> Platform added!");
            }
            else
            {
                Console.WriteLine("--> Platform already exists...");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not add Platform to DB: {ex.Message}");
        }
    }
}

enum EventType
{
    PlatformPublished,
    Undetermined
}

