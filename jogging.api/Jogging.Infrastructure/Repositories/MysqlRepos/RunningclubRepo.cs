using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
using Jogging.Persistence.Context;
using Jogging.Persistence.Models;
using Microsoft.EntityFrameworkCore;
//using Mysqlx;

namespace Jogging.Infrastructure.Repositories.MysqlRepos;

public class RunningclubRepo : IRunningclubRepo
{
    private JoggingContext _db;


    public RunningclubRepo(JoggingContext db)
    {
        _db = db;
    }

    public async Task<List<Runningclub>> GetAllAsync()
    {
        var clubs = await _db.Runningclubs.ToListAsync();

        if (clubs == null || !clubs.Any())
        {
            throw new ArgumentException("No running clubs found");
        }

        return clubs;
    }

    public async Task<List<RunningClubWithData>> GetAllWithDataAsync()
    {
        var clubs = await _db.Runningclubs
            .Include(rc => rc.People)
                .ThenInclude(person => person.Registrations)
                    .ThenInclude(registration => registration.CompetitionPerCategory)
                        .ThenInclude(cpc => cpc.AgeCategory)
            .Select(rc => new RunningClubWithData {
                RunningClubId = rc.Id,
                RunningClubName = rc.Name,
                RunningClubLogo = rc.Logo,
                People = rc.People.Select(person => new PersonData {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    Gender = person.Gender,
                    BirthDate = person.BirthDate,
                    BestRunTimes = person.Registrations
                    .GroupBy(reg => new { reg.CompetitionPerCategory.AgeCategoryId, reg.CompetitionPerCategory.AgeCategory.Name, reg.CompetitionPerCategory.Gender, reg.CompetitionPerCategory.DistanceName })
                    .Select(group => new BestRunTimeDto {
                        AgeCategoryId = group.Key.AgeCategoryId,
                        AgeCategoryName = group.Key.Name,
                        Gender = group.Key.Gender,
                        BestTime = group.Min(reg => reg.RunTime),
                        Distance = group.Key.DistanceName
                    }).ToList()
                }).ToList()
            })
            .ToListAsync();

        if (clubs == null || !clubs.Any())
        {
            throw new ArgumentException("No running clubs found");
        }

        return clubs;        
    }

    public async Task<Runningclub> GetByIdAsync(int id)
    {
        var club = await _db.Runningclubs.FindAsync(id);
        if (club == null)
        {
            throw new ArgumentException("No running club found");
        }

        return club;
    }

    public async Task<Runningclub> GetByNameAsync(string name)
    {
        var club = await _db.Runningclubs.FirstOrDefaultAsync(x => x.Name == name);

        if (club == null)
        {
            throw new ArgumentException("Club with that name doesn't exist");
        }

        return club;
    }

    public async Task<Runningclub> GetByUserIdAsync(int userId)
    {
        var runningclubId = _db.People.Find(userId).RunningClubId;

        if (runningclubId == null)
        {
            throw new ArgumentException("User is not part of any running club");
        }
        
        var runningclub = await _db.Runningclubs.FirstOrDefaultAsync(x => x.Id == runningclubId);
        
        if (runningclub == null)
        {
            throw new ArgumentException("No running club found");
        }
        return runningclub;
    }

    public async Task AddAsync(Runningclub runningclub)
    {
        var existingClub = await _db.Runningclubs.FirstOrDefaultAsync(x => x.Name == runningclub.Name);

        if (existingClub != null)
        {
            throw new ArgumentException("A running club with the same name already exists");
        }
        
        _db.Runningclubs.Add(runningclub);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(int runningId, Runningclub runningclub)
    {
        var existingClub = await GetByIdAsync(runningId);

        if (existingClub == null)
        {
            throw new ArgumentException("No running club found");
        }
        
        existingClub.Name = runningclub.Name;
        existingClub.Url = runningclub.Url;
        existingClub.Logo = runningclub.Logo;
        existingClub.AdminChecked = runningclub.AdminChecked;
        _db.Runningclubs.Update(existingClub);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var club = await GetByIdAsync(id);

        if (club == null)
        {
            throw new ArgumentException("No running club found");
        }
        _db.Runningclubs.Remove(club);
        await _db.SaveChangesAsync();
    }

    public async Task RegisterPersonToClubAsync(int userId, int clubId)
    {
        var runningClub = await _db.Runningclubs.FindAsync(clubId);
        if(runningClub == null)
        {
            throw new ArgumentException("No running club found");
        }

        var person = await _db.People.FindAsync(userId);

        if(person == null)
        {
            throw new ArgumentException("No person found");
        }

        if(person.RunningClubId == clubId)
        {
            throw new ArgumentException("Person is already part of this club");
        }

        person.RunningClubId = clubId;
        try
        {
            await _db.SaveChangesAsync();
        }
        catch(Exception e)
        {
            throw new ArgumentException("Error registering person to club");
        }
    }
}