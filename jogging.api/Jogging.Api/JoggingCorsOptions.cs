namespace Jogging.Api;

public class JoggingCorsOptions
{
    public required string AllowedOrigins { get; set; }
    public required string AllowedMethods { get; set; }
    public required string AllowedHeaders { get; set; }
}
