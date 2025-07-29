namespace Jogging.Persistence.Models
{
    public class RunningClubWithData
    {
       // public Runningclub Runningclub { get; set; }
       public int RunningClubId { get; set; }
        public byte[]? RunningClubLogo { get; set; }
        public string? RunningClubName { get; set; }
        public List<PersonData>? People { get; set; }
    }
}
