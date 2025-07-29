using AutoMapper;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Helpers;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
using Jogging.Persistence.Context;
using Jogging.Persistence.Models;
using Jogging.Persistence.Models.CompetitionResult;
using Jogging.Persistence.Models.Registration;
using Jogging.Persistence.Models.Result;
using Jogging.Persistence.Models.SearchModels.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Jogging.Infrastructure.Repositories.MysqlRepos;

public class ResultRepo : IResultRepo
{
    private readonly JoggingContext _dbJoggingContext;
    private readonly CustomMemoryCache _memoryCache;
    private readonly IMapper _mapper;

    public ResultRepo(JoggingContext joggingContext, CustomMemoryCache memoryCache, IMapper mapper)
    {
        _dbJoggingContext = joggingContext;
        _memoryCache = memoryCache;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ResultFunctionDom>> GetAllResults()
    {
        var allResults = await _dbJoggingContext.Set<ExtendedResultFunctionResponse>()
            .FromSqlRaw("CALL get_competition_results()")
            .ToListAsync();


        if (allResults == null || allResults.Count == 0)
        {
            throw new ResultException("No results found");
        }

        return _mapper.Map<IEnumerable<ResultFunctionDom>>(allResults);
    }

    public async Task<List<CompetitionResultData>> GetAllResultsData()
    {

        var competitionResults = _dbJoggingContext.Competitions
            .Where(c => c.Active == true && c.RankingActive == true)
            .Select(c => new CompetitionResultData {
                CompetitionId = c.Id,
                Distances = c.Competitionpercategories.Select(cpc => new DistanceResult {
                    DistanceName = cpc.DistanceName,
                    GunTime = cpc.GunTime,
                    AgeCategory = cpc.AgeCategory.Name,
                    Participants = cpc.Registrations
                    .Where(r => r.RunTime != null && r.Person != null)
                    .OrderBy(r => r.RunTime)
                    .Select(r => new ParticipantResult {
                        Clublogo = r.Person.RunningClub.Logo,
                        FirstName = r.Person.FirstName,
                        LastName = r.Person.LastName,
                        City = r.Person.Address.City,
                        RunTime = r.RunTime
                        
                    })
                    .ToList()
                }).ToList()
            }).ToList();

        return competitionResults;

    }

    public async Task<IQueryable<ResultDom>> GetPersonResultByIdAsync(int personId)
    {
        var results = await _dbJoggingContext.ExtendedRegistrations
            .Where(r => r.PersonId == personId)
            .Where(r => r.RunTime != null)
            .Where(r => r.RunNumber != null)
            .Join(_dbJoggingContext.Competitionpercategories, r => r.CompetitionPerCategoryId, cpc => cpc.Id,
            (r, cpc) => new {
                r.Id,
                r.RunTime,
                r.RunNumber,
                r.Paid,
                r.CompetitionId,
                r.PersonId,
                cpc.Gender,
                cpc.DistanceName,
                r.CompetitionPerCategoryId
            })
            .Join(_dbJoggingContext.Competitions, rc => rc.CompetitionId, c => c.Id,
            (rc, c) => new {
                rc.Id,
                rc.RunTime,
                rc.RunNumber,
                rc.Paid,
                rc.PersonId,
                rc.Gender,
                rc.DistanceName,
                CompetitionName = c.Name,
                CompetitionDate = c.Date,
                rc.CompetitionPerCategoryId
            })
            .Join(_dbJoggingContext.People, rc => rc.PersonId, p => p.Id,
            (rc, p) => new ExtendedResult {
                CompetitionId = rc.Id,
                RunTime = TimeSpan.Parse(rc.RunTime),
                PersonId = p.Id,
                RunNumber = rc.RunNumber ?? 0,
                CompetitionPerCategoryId = rc.CompetitionPerCategoryId ?? 0,
                Paid = rc.Paid,
            })
            .ToListAsync();


        if (results == null || results.Count == 0)
        {
            throw new ResultException("No results found");
        }

        return _mapper.Map<List<ResultDom>>(results).AsQueryable();
    }

    public async Task<IQueryable<ResultDom>> GetCompetitionResultByIdAsync(int competitionId)
    {
        var queryResults = await _dbJoggingContext.ExtendedRegistrations
            .Where(r => r.CompetitionId == competitionId)
            .Where(r => r.RunNumber != null)
            .Join(_dbJoggingContext.People, r => r.PersonId, p => p.Id,
            (r, p) => new {
                r.Id,
                r.RunTime,
                r.RunNumber,
                r.CompetitionId,
                r.PersonId,
                r.Paid,
                p.FirstName,
                p.LastName
            })
            .Join(_dbJoggingContext.Competitionpercategories, rp => rp.CompetitionId, rpc => rpc.Id,
            (rp, rpc) => new {
                rp.Id,
                rp.RunTime,
                rp.RunNumber,
                rp.PersonId,
                rp.CompetitionId,
                rp.Paid,
                CompetitionPerCategoryId = rpc.Id
            })

            .ToListAsync();


        var results = queryResults.Select(result => new SimpleResult {
            CompetitionId = result.CompetitionId ?? throw new ResultException("Competition was not found"),
            RunTime = TimeSpan.Parse(result.RunTime),
            PersonId = result.PersonId ?? throw new ResultException("Person was not found"),
            CompetitionPerCategoryId = result.CompetitionPerCategoryId,
            RunNumber = result.RunNumber ?? 0,
            Paid = result.Paid
        }).ToList();

        if (results.Count <= 0)
        {
            throw new ResultException("No results found");
        }

        var resultsQuery = _mapper.Map<List<ResultDom>>(results).AsQueryable();
        return resultsQuery;
    }


    //public async Task<IQueryable<ResultDom>> GetCompetitionResultByIdWithRunNumberAsync(int competitionId)
    //{



    //    return results.AsQueryable();

    //}

    public async Task<IQueryable<ResultDom>> GetCompetitionResultByIdWithRunNumberAsync(int competitionId)
    {
        var query = await _dbJoggingContext.ExtendedRegistrations
            .Where(r => r.CompetitionId == competitionId)
            .Where(r => r.RunNumber != null)
            .Where(r => r.RunTime != null)
            .OrderBy(r => r.RunTime)
            .ToListAsync();

        var results = query.Select(r => new ExtendedCompetitionResult {
            CompetitionId = r.CompetitionId ?? throw new ResultException("CompetitionId is null"),
            RunTime = r.RunTime != null ? TimeSpan.Parse(r.RunTime) : throw new ResultException("The runtime was not valid."),
            RunNumber = r.RunNumber,
            PersonId = r.PersonId ?? throw new ResultException("The id of the user could not be found."),
            Paid = r.Paid
        }).ToList();

        if (results.Count <= 0)
        {
            throw new ResultException("No valid results found");
        }

        var resultsQuery = _mapper.Map<List<ResultDom>>(results).AsQueryable();
        return resultsQuery;
    }
    public async Task UpsertBulkAsync(List<ResultDom> registrations)
    {
        var simpleResults = _mapper.Map<List<SimpleResult>>(registrations);
        foreach (var result in simpleResults)
        {
            var existingResult = await _dbJoggingContext.ExtendedRegistrations
                .FirstOrDefaultAsync(r => r.Id == result.Id);

            if (existingResult != null)
            {
                _dbJoggingContext.Entry(existingResult).CurrentValues.SetValues(result);
            }
            else
            {
                var newRegistration = new ExtendedRegistration {
                    Id = result.Id,
                    RunTime = result.RunTime.ToString(),
                    RunNumber = result.RunNumber,
                    PersonId = result.PersonId,
                    CompetitionId = result.CompetitionId,
                    Paid = result.Paid ?? throw new ResultException("The value for Paid is null")
                };
                await _dbJoggingContext.ExtendedRegistrations.AddAsync(newRegistration);
            }
        }

        await _dbJoggingContext.SaveChangesAsync();
    }

    public async Task UpdateRunTimeAsync(int registrationId, ResultDom updatedResult)
    {
        await using var transaction = await _dbJoggingContext.Database.BeginTransactionAsync();
        try
        {
            var registration = await _dbJoggingContext.ExtendedRegistrations
                .FirstOrDefaultAsync(r => r.Id == registrationId);

            if (registration == null)
            {
                throw new ResultException("Result not found");
            }

            registration.RunTime = updatedResult.RunTime.ToString();
            _dbJoggingContext.ExtendedRegistrations.Update(registration);
            await _dbJoggingContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        //_memoryCache.Remove(CacheKeyGenerator.GetCompetitionResultsKey(registration.CompetitionId));
        //_memoryCache.Remove(CacheKeyGenerator.GetAllResultsKey());
    }
}