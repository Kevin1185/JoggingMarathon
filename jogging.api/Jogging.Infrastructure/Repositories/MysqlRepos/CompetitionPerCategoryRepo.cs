using AutoMapper;
using Jogging.Domain.Enums;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
//using Jogging.Infrastructure.Models.DatabaseModels.CompetitionPerCategory;
using Jogging.Persistence.Context;
using Jogging.Persistence.Models.CompetitionPerCategory;
using Microsoft.EntityFrameworkCore;
//using Postgrest;
//using Client = Supabase.Client;

namespace Jogging.Infrastructure.Repositories.MysqlRepos;

public class CompetitionPerCategoryRepo : ICompetitionPerCategoryRepo
{
    private readonly JoggingContext _dbJoggingContext;
    private readonly IAgeCategoryRepo _ageCategoryRepo;
    private readonly IMapper _mapper;

    public CompetitionPerCategoryRepo(JoggingContext joggingContext, IAgeCategoryRepo ageCategoryRepo, IMapper mapper)
    {
        _dbJoggingContext = joggingContext;
        _ageCategoryRepo = ageCategoryRepo;
        _mapper = mapper;
    }

    public async Task<List<CompetitionPerCategoryDom>> UpdateAsync(Dictionary<string, float> distances, int competitionId)
    {
        var competitionPerCategories = await _dbJoggingContext.Competitionpercategories
            .Where(c => c.CompetitionId == competitionId)
            .ToListAsync();

        foreach (var keyValuePair in distances.OrderBy(c => c.Value))
        {
            var competitionPerCategory = competitionPerCategories
                .FirstOrDefault(c => c.DistanceName == keyValuePair.Key);

            if (competitionPerCategory != null)
            {
                competitionPerCategory.DistanceInKm = keyValuePair.Value;
            }
        }

        await _dbJoggingContext.SaveChangesAsync();

        return _mapper.Map<List<CompetitionPerCategoryDom>>(competitionPerCategories);
    }

    public async Task DeleteAsync(int competitionPerCategoryId)
    {
        var competitionPerCategory = await _dbJoggingContext.Competitionpercategories
            .FirstOrDefaultAsync(c => c.Id == competitionPerCategoryId);
        _dbJoggingContext.Competitionpercategories.Remove(competitionPerCategory);
        
        await _dbJoggingContext.SaveChangesAsync();
    }

    public async Task<List<CompetitionPerCategoryDom>> AddAsync(Dictionary<string, float> distances,
        int competitionId)
    {
        var ageCategories = await _ageCategoryRepo.GetAllAsync();
        List<ExtendedCompetitionpercategory> competitionPerCategories = new List<ExtendedCompetitionpercategory>();

        ageCategories.ToList().ForEach(a => {
            foreach (var keyValuePair in distances.OrderBy(c => c.Value))
            {
                foreach (Char gender in (Genders[])Enum.GetValues(typeof(Genders)))
                {
                    var competitionPerCategory = new ExtendedCompetitionpercategory() {
                        DistanceName = keyValuePair.Key,
                        DistanceInKm = keyValuePair.Value,
                        AgeCategoryId = a.Id,
                        CompetitionId = competitionId,
                        Gender = gender.ToString()
                    };
                    competitionPerCategories.Add(competitionPerCategory);
                }
            }
        });

        await _dbJoggingContext.Competitionpercategories.AddRangeAsync(competitionPerCategories);
        await _dbJoggingContext.SaveChangesAsync();

        return _mapper.Map<List<CompetitionPerCategoryDom>>(competitionPerCategories);
    }

    public async Task UpdateGunTimeAsync(int competitionId, DateTime gunTime)
    {
        var competitionPerCategories = await GetByCompetitionIdAsync(competitionId).ToListAsync();

        if (competitionPerCategories.Count == 0)
        {
            throw new CompetitionException("Competition not found");
        }

        foreach (var competitionPerCategory in competitionPerCategories)
        {
            competitionPerCategory.GunTime = gunTime;
        }

        _dbJoggingContext.Competitionpercategories.UpdateRange(competitionPerCategories);
        await _dbJoggingContext.SaveChangesAsync();
    }
    

public async Task<CompetitionPerCategoryDom> GetCompetitionPerCategoryByParameters(int ageCategoryId, string distanceName, char personGender,
        int competitionId)
    {
        var competitionPerCategory = await _dbJoggingContext.Competitionpercategories
            .FirstOrDefaultAsync(cPc => cPc.AgeCategoryId == ageCategoryId &&
                                        cPc.DistanceName == distanceName &&
                                        cPc.CompetitionId == competitionId &&
                                        cPc.Gender == personGender.ToString());

        if (competitionPerCategory == null)
        {
            throw new CompetitionException("This competition per category doesn't exist");
        }

        return _mapper.Map<CompetitionPerCategoryDom>(competitionPerCategory);
    }

    private IQueryable<ExtendedCompetitionpercategory> GetByCompetitionIdAsync(int competitionId)
    {
        return _dbJoggingContext.Competitionpercategories
            .Where(cpc => cpc.CompetitionId == competitionId);
    }

    
    public Task<CompetitionPerCategoryDom> AddAsync(CompetitionPerCategoryDom person)
    {
        throw new NotImplementedException();
    }

    public Task<List<CompetitionPerCategoryDom>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<CompetitionPerCategoryDom> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<CompetitionPerCategoryDom> UpdateAsync(int id, CompetitionPerCategoryDom updatedItem)
    {
        throw new NotImplementedException();
    }

    public Task<CompetitionPerCategoryDom> UpsertAsync(int? id, CompetitionPerCategoryDom updatedItem)
    {
        throw new NotImplementedException();
    }
}