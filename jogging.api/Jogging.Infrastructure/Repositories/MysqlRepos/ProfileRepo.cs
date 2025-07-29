using AutoMapper;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
using Jogging.Domain.Validators;
using Jogging.Persistence.Context;

namespace Jogging.Infrastructure.Repositories.MysqlRepos;

public class ProfileRepo : IProfileRepo
{
    private readonly JoggingContext _dbJoggingContext;
    private readonly IMapper _mapper;

    public ProfileRepo(JoggingContext joggingContext, IMapper mapper)
    {
        _dbJoggingContext = joggingContext;
        _mapper = mapper;
    }

    public async Task UpdateAsync(string userId, ProfileDom updatedItem)
    {
        var test =  _dbJoggingContext.Profiles;
        var profileId = Guid.Parse(userId);
        var profile = await _dbJoggingContext.Profiles.FindAsync(profileId);
        if (profile == null)
        {
            throw new ProfileException("Profile not found.");
        }

        profile.Role = updatedItem.Role;

        _dbJoggingContext.Profiles.Update(profile);
        var result = await _dbJoggingContext.SaveChangesAsync();

        if (result == 0)
        {
            throw new ProfileException("Error while updating user role.");
        }
    }
}
