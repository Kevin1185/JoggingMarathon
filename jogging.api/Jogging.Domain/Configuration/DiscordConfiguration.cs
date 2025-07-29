namespace Jogging.Domain.Configuration;

public class DiscordConfiguration
{
    public ulong WebhookId { get; set; }
    public required string WebhookToken { get; set; }
}