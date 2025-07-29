namespace Jogging.Infrastructure.Models.SearchModels.Account;

public class SupabaseUser
{
    public Guid? Id { get; set; }
    public DateTime? EmailConfirmedAt { get; set; }
    public string? Email { get; set; }
}