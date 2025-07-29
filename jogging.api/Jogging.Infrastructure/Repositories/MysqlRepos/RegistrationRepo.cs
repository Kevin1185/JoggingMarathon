using AutoMapper;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Helpers;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
using Jogging.Persistence.Context;
using Jogging.Persistence.Models.Registration;
using Microsoft.EntityFrameworkCore;

namespace Jogging.Infrastructure.Repositories.MysqlRepos
{
    public class RegistrationRepo : IRegistrationRepo
    {
        private readonly JoggingContext _dbJoggingContext;
        private readonly IAgeCategoryRepo _ageCategoryRepo;
        private readonly IPersonRepo _personRepo;
        private readonly ICompetitionPerCategoryRepo _competitionPerCategoryRepo;
        private readonly ICompetitionRepo _competitionRepo;
        private readonly IMapper _mapper;
        private readonly CustomMemoryCache _memoryCache;

        public RegistrationRepo(JoggingContext joggingContext, IAgeCategoryRepo ageCategoryRepo,
            ICompetitionPerCategoryRepo competitionPerCategoryRepo, IMapper mapper, CustomMemoryCache memoryCache, ICompetitionRepo competitionRepo,
            IPersonRepo personRepo)
        {
            _dbJoggingContext = joggingContext;
            _ageCategoryRepo = ageCategoryRepo;
            _competitionPerCategoryRepo = competitionPerCategoryRepo;
            _mapper = mapper;
            _memoryCache = memoryCache;
            _competitionRepo = competitionRepo;
            _personRepo = personRepo;
        }

        #region GetRegistrationByPersonIdAndCompetitionIdAsync

        public async Task<RegistrationDom> GetRegistrationByPersonIdAndCompetitionIdAsync(int personId, int competitionId, bool throwError = true)
        {
            var personRegistration = await _dbJoggingContext.PersonRegistrations
                .Where(pr => pr.PersonId == personId)
                .Where(pr => pr.CompetitionId == competitionId)
                .FirstOrDefaultAsync();

            if (throwError && personRegistration == null)
            {
                throw new RegistrationNotFoundException("Registration not found");
            }

            return _mapper.Map<RegistrationDom>(personRegistration);
        }

        public async Task<List<RegistrationDom>> GetAllAsync()
        {
            var result = await _dbJoggingContext.ExtendedRegistrations.ToListAsync();

            if (result.Count <= 0)
            {
                throw new RegistrationNotFoundException("No registrations found");
            }

            return _mapper.Map<List<RegistrationDom>>(result);
        }

        #endregion

        #region GetAllAsync (withRunNumber)

        public async Task<List<RegistrationDom>> GetAllAsync(bool withRunNumber)
        {
            var registrations = await _dbJoggingContext.ExtendedRegistrations
                .Where(r => r.RunNumber != null)
                .ToListAsync();            

            if (registrations.Count <= 0)
            {
                throw new RegistrationNotFoundException("No registrations found");
            }

            return _mapper.Map<List<RegistrationDom>>(registrations);
        }

        public async Task<List<RegistrationDom>> GetByCompetitionIdAndSearchValueAsync(int competitionId, string searchValue, bool withRunNumber)
        {
            var storedProcedure = await _dbJoggingContext.ExtendedRegistrationSearchByPeople
                .FromSqlInterpolated($"CALL get_competition_registrations({competitionId}, {searchValue})")
                .ToListAsync();

            if (withRunNumber)
            {
                storedProcedure = storedProcedure.Where(p => p.RunNumber != null).ToList();
            }

            if (storedProcedure == null || storedProcedure.Count == 0)
            {
                throw new RegistrationNotFoundException("No registrations found");
            }

            return _mapper.Map<List<RegistrationDom>>(storedProcedure);
        }

        #endregion

        #region GetRegistrationsByPersonIdAndCompetitionIdAsync

        public async Task<List<RegistrationDom>> GetByPersonIdAndCompetitionIdAsync(int personId, int competitionId, bool withRunNumber,
            bool throwError = true)
        {
            var query = _dbJoggingContext.SimpleRegistrations
                .Where(r => r.PersonId == personId)
                .Where(r => r.CompetitionId == competitionId);
            

            if (withRunNumber)
            {
                query = query.Where(p => p.RunNumber != null);
            }

            var registration = await query
                .Take(1)
                .ToListAsync();

            if (throwError && registration.Count == 0)
            {
                throw new RegistrationNotFoundException("No registrations found");
            }

            var mapped = _mapper.Map<List<RegistrationDom>>(registration);

            return mapped;
        }

        #endregion

        #region GetRegistrationsByPersonIdAsync

        public async Task<List<RegistrationDom>> GetByPersonIdAsync(int personId, bool withRunNumber)
        {
            var query = _dbJoggingContext.ExtendedRegistrations
                .Where(r => r.PersonId == personId);            

            if (withRunNumber)
            {
                query = query.Where(p => p.RunNumber != null);
            }

            var registrations = await query.ToListAsync();

            if (registrations.Count <= 0)
            {
                throw new RegistrationNotFoundException("No registrations found");
            }

            return _mapper.Map<List<RegistrationDom>>(registrations);
        }

        #endregion

        #region GetRegistrationsByCompetitionIdAsync

        public async Task<List<RegistrationDom>> GetByCompetitionIdAsync(int competitionId, bool withRunNumber)
        {
            var query = _dbJoggingContext.ExtendedRegistrations
                .Where(r => r.CompetitionId == competitionId);

            if (withRunNumber)
            {
                query = query.Where(p => p.RunNumber != null);
            }

            var registrations = await query.ToListAsync();

            if (registrations.Count <= 0)
            {
                throw new RegistrationNotFoundException("No registrations found");
            }

            return _mapper.Map<List<RegistrationDom>>(registrations);
        }

        #endregion

        #region DeleteByPersonIdAndCompetitionIdAsync

        public async Task DeleteByPersonIdAndCompetitionIdAsync(int personId, int competitionId)
        {
            var toBeDeletedPersonIdAndCompetitionAd = await _dbJoggingContext.SimpleRegistrations
                .Where(r => r.PersonId == personId)
                .Where(r => r.CompetitionId == competitionId)
                .FirstOrDefaultAsync();
            if(toBeDeletedPersonIdAndCompetitionAd == null)
            {
                throw new RegistrationNotFoundException("Registration not found");
            }

            _dbJoggingContext.SimpleRegistrations.Remove(toBeDeletedPersonIdAndCompetitionAd);
            await _dbJoggingContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int registrationId)
        {
            var registration = await _dbJoggingContext.SimpleRegistrations
                .Where(c => c.Id == registrationId)
                .FirstOrDefaultAsync();

            if (registration == null)
            {
                throw new RegistrationNotFoundException("Registration not found");
            }

            _dbJoggingContext.SimpleRegistrations.Remove(registration);
            await _dbJoggingContext.SaveChangesAsync();
        }

        #endregion

        #region GetByIdAsync

        public async Task<RegistrationDom> GetByIdAsync(int registrationId)
        {
            var registration = await GetSimpleRegistrationByIdAsync(registrationId);

            if (registration == null)
            {
                throw new RegistrationNotFoundException("Registration not found");
            }

            return _mapper.Map<RegistrationDom>(registration);
        }

        #endregion

        #region SignInToContestAsync

        public async Task<RegistrationDom> SignInToContestAsync(int competitionId, PersonDom person, string distanceName)
        {
            await CheckDuplicateRegistration(person.Id, competitionId);
            CompetitionDom competition = await _competitionRepo.GetSimpleCompetitionByIdAsync(competitionId);
            AgeCategoryDom ageCategory = await _ageCategoryRepo.GetAgeCategoryByAge(person);
            CompetitionPerCategoryDom competitionPerCategory =
                await _competitionPerCategoryRepo.GetCompetitionPerCategoryByParameters(ageCategory.Id, distanceName, person.Gender, competitionId);

            ExtendedRegistration registration = new ExtendedRegistration
                { CompetitionPerCategoryId = competitionPerCategory.Id, Paid = false, PersonId = person.Id, CompetitionId = competitionId };

            await _dbJoggingContext.SimpleRegistrations.AddAsync(registration);
            _dbJoggingContext.SaveChanges();

            var performedRegistration = await _dbJoggingContext.SimpleRegistrations
                .Where(r => r.Id == registration.Id).FirstOrDefaultAsync();

            if (performedRegistration == null)
            {
                throw new PersonRegistrationException("Something went wrong doing your registration");
            }
            
            return new RegistrationDom()
            {
                Id = performedRegistration.Id,
                PersonId = person.Id,
                Person =person,
                CompetitionId = competitionId,
                Competition = competition,
                CompetitionPerCategoryId = competitionPerCategory.Id,
                CompetitionPerCategory = competitionPerCategory,
                Paid = false,
            };
        }

        #endregion

        #region UpdateAsync

        public async Task<RegistrationDom> UpdateAsync(int registrationId, RegistrationDom updatedItem)
        {
            var oldRegistration = await GetSimpleRegistrationByIdAsync(registrationId);

            if (oldRegistration == null)
            {
                throw new RegistrationNotFoundException("Registration not found");
            }

            if (updatedItem.RunNumber != null)
            {
                var runNumber = updatedItem.RunNumber == -1 ? null : updatedItem.RunNumber;
                oldRegistration.RunNumber = runNumber;
            }

            if (updatedItem.Paid != null)
            {
                oldRegistration.Paid = updatedItem.Paid ?? throw new Exception("The 'Paid' field of the updated registration was empty.") ;
            }
            _dbJoggingContext.SimpleRegistrations.Update(oldRegistration);
            await _dbJoggingContext.SaveChangesAsync();

            return _mapper.Map<RegistrationDom>(oldRegistration);
        }

        #endregion

        #region UpdateRunNumberAsync

        public async Task<RegistrationDom> UpdateRunNumberAsync(int registrationId, RegistrationDom updatedItem)
        {
            var oldRegistration = await GetSimpleRegistrationByIdAsync(registrationId);

            if (oldRegistration == null)
            {
                throw new RegistrationNotFoundException("Registration not found");
            }

            var runNumber = updatedItem.RunNumber == -1 ? null : updatedItem.RunNumber;
            oldRegistration.RunNumber = runNumber;

            _dbJoggingContext.SimpleRegistrations.Update(oldRegistration);
            await _dbJoggingContext.SaveChangesAsync();

            return _mapper.Map<RegistrationDom>(oldRegistration);
        }

        #endregion

        #region UpdatePaidAsync

        public async Task<RegistrationDom> UpdatePaidAsync(int registrationId, RegistrationDom updatedItem)
        {
            var oldRegistration = await GetSimpleRegistrationByIdAsync(registrationId);

            if (oldRegistration == null)
            {
                throw new RegistrationNotFoundException("Registration not found");
            }

            if (oldRegistration.Paid != updatedItem.Paid)
            {
                oldRegistration.Paid = updatedItem.Paid ?? throw new Exception("The 'Paid' field of the updated registration was empty.");

                _dbJoggingContext.SimpleRegistrations.Update(oldRegistration);
                await _dbJoggingContext.SaveChangesAsync();
            }

            return _mapper.Map<RegistrationDom>(oldRegistration);
        }

        #endregion

        #region UpdateCompetitionPerCategoryAsync

        public async Task<RegistrationDom> UpdateCompetitionPerCategoryAsync(int registrationId, int personId,
            CompetitionPerCategoryDom competitionPerCategory)
        {
            var oldRegistration = await GetSimpleRegistrationByIdAsync(registrationId);
            if (oldRegistration == null)
            {
                throw new RegistrationNotFoundException("Registration not found");
            }

            if (oldRegistration.PersonId != personId)
            {
                throw new PersonRegistrationException("You can't change this registration");
            }

            var person = await _personRepo.GetByIdAsync(personId);
            var ageCategory = await _ageCategoryRepo.GetAgeCategoryByAge(person);

            var newCompetitionPerCategory = await _competitionPerCategoryRepo
                .GetCompetitionPerCategoryByParameters(
                    ageCategory.Id,
                    competitionPerCategory.DistanceName,
                    person.Gender,
                    oldRegistration.CompetitionId ?? throw new Exception("The 'CompetitionId' field of the updated registration was empty.")
                );
            
            oldRegistration.CompetitionPerCategoryId = newCompetitionPerCategory.Id;

            _dbJoggingContext.SimpleRegistrations.Update(oldRegistration);
            await _dbJoggingContext.SaveChangesAsync();
            
            //_memoryCache.Remove(CacheKeyGenerator.GetCompetitionResultsKey(oldRegistration.CompetitionId));
            //_memoryCache.Remove(CacheKeyGenerator.GetAllResultsKey());

            return _mapper.Map<RegistrationDom>(oldRegistration);
        }

        #endregion

        #region GetSimpleRegistrationByIdAsync

        private async Task<SimpleRegistration?> GetSimpleRegistrationByIdAsync(int registrationId)
        {
            var registration = await _dbJoggingContext.SimpleRegistrations
                .Where(c => c.Id == registrationId)
                .FirstOrDefaultAsync();
            return registration;
        }

        #endregion

        #region CheckDuplicateRegistration

        private async Task CheckDuplicateRegistration(int personId, int competitionId)
        {
            var existingRegistrations = await _dbJoggingContext.SimpleRegistrations
                .Where(pr => pr.PersonId == personId)
                .Where(pr => pr.CompetitionId == competitionId)
                .FirstOrDefaultAsync();            

            if (existingRegistrations != null)
            {
                throw new RegistrationAlreadyExistsException("Deze registratie bestaat al");
            }
        }

        #endregion

        #region UpsertAsync

        public Task<RegistrationDom> UpsertAsync(int? id, RegistrationDom updatedItem)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region AddAsync

        public Task<RegistrationDom> AddAsync(RegistrationDom person)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}