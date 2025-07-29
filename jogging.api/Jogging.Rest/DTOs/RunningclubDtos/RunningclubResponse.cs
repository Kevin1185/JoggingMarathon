namespace Jogging.Rest.DTOs.RunningclubDtos;

public class RunningclubResponse
{
    public int Id { get; set; }
    public string Name {get; set;}
    public string Url {get; set;}
    public bool AdminChecked { get; set; }
    public byte[] Logo { get; set; }
}