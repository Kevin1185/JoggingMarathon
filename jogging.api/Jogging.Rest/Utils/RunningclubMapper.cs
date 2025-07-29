using Jogging.Persistence.Models;
using Jogging.Rest.DTOs.RunningclubDtos;

namespace Jogging.Domain.Helpers;

public static class RunningclubMapper
{
    public static Runningclub Map(this RunningclubRequest contract)
    {
        return new Runningclub()
        {
            Name = contract.Name,
            AdminChecked = contract.AdminChecked,
            Logo = contract.Logo,
            Url = contract.Url
        };
    }

    public static RunningclubResponse Map(this Runningclub contract)
    {
        return new RunningclubResponse()
        {
            Id = contract.Id,
            Name = contract.Name,
            AdminChecked = contract.AdminChecked,
            Logo = contract.Logo,
            Url = contract.Url
        };
    }
    
}