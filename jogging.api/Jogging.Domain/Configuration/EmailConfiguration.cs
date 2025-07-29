namespace Jogging.Domain.Configuration;

public class EmailConfiguration
{
    public required string Host { get; set; }
    public int Port { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}