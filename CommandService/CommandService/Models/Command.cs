namespace CommandService.Models;
public class Command
{
    public int Id { get; set; }

    public string HowTo { get; set; } = default!;

    public string CommandLine { get; set; } = default!;

    public int PlatformId { get; set; }

    public Platform Platform { get; set; } = default!;
}
