using AutoMapper;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
//using Jogging.Infrastructure.Models.DatabaseModels.AgeCategory;
using Jogging.Persistence.Context;
using Microsoft.EntityFrameworkCore;


namespace Jogging.Infrastructure.Repositories.MysqlRepos;

public class AgeCategoryRepo : IAgeCategoryRepo
{
    private readonly JoggingContext _dbJoggingContext;
    private readonly IMapper _mapper;

    public AgeCategoryRepo(JoggingContext joggingContext, IMapper mapper)
    {
        _dbJoggingContext = joggingContext;
        _mapper = mapper;
    }

    public async Task<List<AgeCategoryDom>> GetAllAsync()
    {
        var ageCategories = await _dbJoggingContext.Agecategories.ToListAsync();

        if (ageCategories.Count <= 0)
        {
            throw new AgeCategoryException("No age categories found");
        }

        return _mapper.Map<List<AgeCategoryDom>>(ageCategories);
    }

    public async Task<AgeCategoryDom> GetByIdAsync(int ageCategoryId)
    {
        var ageCategory = await _dbJoggingContext.Agecategories
            .FirstOrDefaultAsync(ac => ac.Id == ageCategoryId);

        if (ageCategory == null)
        {
            throw new AgeCategoryException("Age category not found");
        }

        return _mapper.Map<AgeCategoryDom>(ageCategory);
    }

    public async Task<AgeCategoryDom> GetAgeCategoryByAge(PersonDom person)
    {
        var ageCategory = await _dbJoggingContext.Agecategories
            .FirstOrDefaultAsync(ageCat => ageCat.MaximumAge >= person.BirthYearAge && ageCat.MinimumAge <= person.BirthYearAge);

        if (ageCategory == null)
        {
            throw new AgeCategoryException("Age category not found");
        }

        return _mapper.Map<AgeCategoryDom>(ageCategory);
    }

    public Task<AgeCategoryDom> AddAsync(AgeCategoryDom person)
    {
        throw new NotImplementedException();
    }


    public Task<AgeCategoryDom> UpdateAsync(int id, AgeCategoryDom updatedItem)
    {
        throw new NotImplementedException();
    }

    public Task<AgeCategoryDom> UpsertAsync(int? id, AgeCategoryDom updatedItem)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}