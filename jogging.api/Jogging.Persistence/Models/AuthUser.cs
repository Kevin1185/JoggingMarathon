namespace Jogging.Persistence.Models;

public partial class AuthUser
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
}
