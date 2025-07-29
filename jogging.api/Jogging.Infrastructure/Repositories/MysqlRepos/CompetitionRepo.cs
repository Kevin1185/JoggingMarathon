using AutoMapper;
using Jogging.Domain.DomainManagers;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Helpers;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
using Jogging.Persistence.Context;

//using Jogging.Infrastructure.Models.DatabaseModels.Address;
//using Jogging.Infrastructure.Models.DatabaseModels.Competition;
//using Jogging.Infrastructure.Models.DatabaseModels.CompetitionPerCategory;
using Jogging.Persistence.Models.Address;
using Jogging.Persistence.Models.Competition;
using Jogging.Persistence.Models.CompetitionPerCategory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Jogging.Infrastructure.Repositories.MysqlRepos
{
    public class CompetitionRepo : ICompetitionRepo
    {
        private readonly JoggingContext _dbJoggingContext;
        private readonly IGenericRepo<AddressDom> _addressRepo;
        private readonly ICompetitionPerCategoryRepo _competitionPerCategoryRepo;
        private readonly IMapper _mapper;
        private readonly CustomMemoryCache _memoryCache;

        public CompetitionRepo(JoggingContext joggingContext, IGenericRepo<AddressDom> addressRepo, ICompetitionPerCategoryRepo competitionPerCategoryRepo,
            IMapper mapper, CustomMemoryCache memoryCache)
        {
            _dbJoggingContext = joggingContext;
            _addressRepo = addressRepo;
            _competitionPerCategoryRepo = competitionPerCategoryRepo;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        public async Task<List<CompetitionDom>> GetAllAsync()
        {
            var competitions = await _dbJoggingContext.Competitions
                .OrderBy(competition => competition.Date)
                .ToListAsync();

            if (competitions.Count <= 0)
            {
                throw new CompetitionException("No competitions found");
            }

            return _mapper.Map<List<CompetitionDom>>(competitions);
        }


        public async Task<List<CompetitionDom>> GetAllWithSearchValuesAsync(string? competitionName, DateOnly? startDate, DateOnly? endDate)
        {
            var query = _dbJoggingContext.Competitions.AsQueryable();

            if (!string.IsNullOrEmpty(competitionName))
            {
                query = query.Where(c => EF.Functions.Like(c.Name, $"%{competitionName}%"));
            }

            if (startDate.HasValue)
            {
                var startDateTime = startDate.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(c => c.Date >= startDateTime);
            }

            if (endDate.HasValue)
            {
                var endDateTime = endDate.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(c => c.Date <= endDateTime);
            }

            var competitions = await query.OrderBy(c => c.Date).ToListAsync();

            if (competitions.Count <= 0)
            {
                throw new CompetitionException("No competitions found");
            }

            return _mapper.Map<List<CompetitionDom>>(competitions);
        }
        public async Task<List<CompetitionDom>> GetAllActiveAsync()
        {
            var competitions = await _dbJoggingContext.Competitions.ToListAsync();
            var selectedCompetitions = competitions
                .Where(c => c.Active == true)
                .OrderBy(c => c.Date);
                //.ToListAsync();

            if (competitions.Count <= 0)
            {
                throw new CompetitionException("No competitions found");
            }

            return _mapper.Map<List<CompetitionDom>>(competitions);
        }

        public async Task<List<CompetitionDom>> GetAllActiveWithSearchValuesAsync(string? competitionName, DateOnly? startDate, DateOnly? endDate)
        {
            var query = _dbJoggingContext.Competitions
                .Where(c => c.Active == true)
                .AsQueryable();

            if (!string.IsNullOrEmpty(competitionName))
            {
                query = query.Where(c => EF.Functions.Like(c.Name, $"%{competitionName}%"));
            }

            if (startDate.HasValue)
            {
                var startDateTime = startDate.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(c => c.Date >= startDateTime);
            }

            if (endDate.HasValue)
            {
                var endDateTime = endDate.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(c => c.Date <= endDateTime);
            }

            var competitions = await query.OrderBy(c => c.Date).ToListAsync();

            if (competitions.Count <= 0)
            {
                throw new CompetitionException("No competitions found");
            }

            return _mapper.Map<List<CompetitionDom>>(competitions);
        }

        public async Task<List<CompetitionDom>> GetAllActiveRankingAsync()
        {
            var competitions = await _dbJoggingContext.Competitions
                .Where(c => c.RankingActive == true)
                .OrderBy(c => c.Date)
                .ToListAsync();

            if (competitions.Count <= 0)
            {
                throw new CompetitionException("No competitions found");
            }

            return _mapper.Map<List<CompetitionDom>>(competitions);
        }

        //TODO: fix for simple and extended!!!!! Do we just map with the mapper? (see extended)
        public async Task<CompetitionDom> GetSimpleCompetitionByIdAsync(int competitionId)
        {
            var competition = await _dbJoggingContext.Competitions
                .Where(c => c.Id == competitionId)
                .SingleOrDefaultAsync();

            if (competition == null)
            {
                throw new CompetitionException("Competition not found");
            }

            return _mapper.Map<CompetitionDom>(competition);
        }

        public async Task<CompetitionDom> GetByIdAsync(int competitionId)
        {
            var competition = await GetExtendedCompetitionById(competitionId);

            return _mapper.Map<CompetitionDom>(competition);
        }

        public async Task<CompetitionDom> AddAsync(CompetitionDom competitionDom)
        {
            var address = await _addressRepo.UpsertAsync(null, _mapper.Map<AddressDom>(competitionDom.Address));

            competitionDom.AddressId = address.Id;
            competitionDom.Address = address;

            var competitionEntity = _mapper.Map<Persistence.Models.Competition.ExtendedCompetition>(competitionDom);
            await _dbJoggingContext.Competitions.AddAsync(competitionEntity);
            await _dbJoggingContext.SaveChangesAsync();

            if (competitionEntity.Id == 0)
            {
                throw new CompetitionException("Something went wrong adding competition");
            }

            if (competitionDom.Distances != null)
            {
                var competitionPerCategories = await _competitionPerCategoryRepo.AddAsync(competitionDom.Distances, competitionEntity.Id);
                competitionDom.CompetitionPerCategories = competitionPerCategories;
            }

            if (competitionDom.RankingActive)
            {
                _memoryCache.Remove(CacheKeyGenerator.GetAllResultsKey());
            }

            competitionDom.Id = competitionEntity.Id;
            return competitionDom;
        }

        public async Task<CompetitionDom> UpdateAsync(int competitionId, CompetitionDom updatedCompetition)
        {
            var currentCompetition = await GetExtendedCompetitionById(competitionId);

            var isAddressUpdated = await UpdateAddressIfNeeded(currentCompetition, updatedCompetition.Address);
            var isDistancesUpdated = await UpdateDistancesIfNeeded(currentCompetition, updatedCompetition);

            if (isAddressUpdated || isDistancesUpdated || !_mapper.Map<CompetitionDom>(currentCompetition).Equals(updatedCompetition))
            {
                currentCompetition.Name = updatedCompetition.Name;
                currentCompetition.Information = updatedCompetition.Information;
                currentCompetition.Date = updatedCompetition.Date;
                currentCompetition.Active = updatedCompetition.Active;
                if (currentCompetition.RankingActive != updatedCompetition.RankingActive)
                {
                    _memoryCache.Remove(CacheKeyGenerator.GetAllResultsKey());
                }

                currentCompetition.RankingActive = updatedCompetition.RankingActive;

                //var competition = await currentCompetition.Update<ExtendedCompetition>();
                var competition = _dbJoggingContext.Competitions.Update(currentCompetition);
                await _dbJoggingContext.SaveChangesAsync();

                if (competition == null)
                {
                    throw new CompetitionException("Something went wrong while updating your competition");
                }

                return _mapper.Map<CompetitionDom>(competition);
            }

            return _mapper.Map<CompetitionDom>(currentCompetition);
        }

        public async Task DeleteAsync(int competitionId)
        {
            var competition = await _dbJoggingContext.Competitions
                .Where(c => c.Id == competitionId)
                .SingleOrDefaultAsync();

            if (competition == null)
            {
                throw new CompetitionException("Competition not found");
            }

            _dbJoggingContext.Competitions.Remove(competition);
            await _dbJoggingContext.SaveChangesAsync();
        }

        private async Task<ExtendedCompetition> GetExtendedCompetitionById(int competitionId)
        {
            var competition = await _dbJoggingContext.Competitions
                .Include(c => c.Address)
                .Include(c => c.Competitionpercategories)
                .Where(c => c.Id == competitionId)
                .SingleOrDefaultAsync();

            if (competition == null)
            {
                throw new CompetitionException("Competition not found");
            }

            return competition;
        }

        private async Task<bool> UpdateAddressIfNeeded(ExtendedCompetition competition, AddressDom updatedAddress)
        {
            var currentAddress = _mapper.Map<AddressDom>(competition.Address);

            if (!currentAddress.Equals(updatedAddress))
            {
                var address = await _addressRepo.UpsertAsync(null, updatedAddress);
                competition.AddressId = address.Id;
                competition.Address = _mapper.Map<ExtendedAddress>(address);
                return true;
            }

            return false;
        }

        private async Task<bool> UpdateDistancesIfNeeded(ExtendedCompetition competition, CompetitionDom updatedCompetitionDom)
        {
            var existingDistances = CompetitionManager.GetDistances(_mapper.Map<CompetitionDom>(competition));
            if (!DictionaryHelper.AreDictionariesEqual(updatedCompetitionDom.Distances, existingDistances))
            {
                var competitionPerCategories = await _competitionPerCategoryRepo.UpdateAsync(updatedCompetitionDom.Distances, competition.Id);
                competition.Competitionpercategories = _mapper.Map<List<ExtendedCompetitionpercategory>>(competitionPerCategories);
                return true;
            }

            return false;
        }

        public Task<CompetitionDom> UpsertAsync(int? addressId, CompetitionDom updatedItem)
        {
            throw new NotImplementedException();
        }
    }
}