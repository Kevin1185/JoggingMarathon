namespace Jogging.Persistence.Models.Account;

public class PasswordReset
{
    public required string RecoveryToken { get; set; }
    public required string NewPassword { get; set; }
}