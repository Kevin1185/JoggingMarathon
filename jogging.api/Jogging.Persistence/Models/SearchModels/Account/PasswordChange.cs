namespace Jogging.Persistence.Models.Account;

public class PasswordChange
{
    public Guid UserId { get; set; }
    public required string OldPassword { get; set; }
    public required string NewPassword { get; set; }
}