using AutoMapper;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
using Jogging.Persistence.Context;
using Jogging.Persistence.Models.School;
using Microsoft.EntityFrameworkCore;

namespace Jogging.Infrastructure.Repositories.MysqlRepos
{
    public class SchoolRepo : IGenericRepo<SchoolDom>
    {
        private readonly JoggingContext _dbJoggingContext;
        private readonly IMapper _mapper;

        public SchoolRepo(JoggingContext joggingContext, IMapper mapper)
        {
            _dbJoggingContext = joggingContext;
            _mapper = mapper;
        }

        public async Task<List<SchoolDom>> GetAllAsync()
        {
            var schools = await _dbJoggingContext.SimpleSchools
                .AsNoTracking()
                .ToListAsync();

            if (schools == null || !schools.Any())
            {
                throw new SchoolNotFoundException("No schools found");
            }

            return _mapper.Map<List<SchoolDom>>(schools);
        }

        public async Task<SchoolDom> GetByIdAsync(int schoolId)
        {
            var school = await GetSchoolById(schoolId);

            if (school == null)
            {
                throw new SchoolNotFoundException("No school found");
            }

            return _mapper.Map<SchoolDom>(school);
        }

        public async Task<SchoolDom> AddAsync(SchoolDom schoolDom)
        {
            var existingSchool = await GetSchoolByName(schoolDom.Name);

            if (existingSchool != null)
            {
                return _mapper.Map<SchoolDom>(existingSchool);
            }

            var newSchool = _mapper.Map<SimpleSchool>(schoolDom);
            await _dbJoggingContext.SimpleSchools.AddAsync(newSchool);
            await _dbJoggingContext.SaveChangesAsync();

            return _mapper.Map<SchoolDom>(newSchool);
        }

        public async Task<SchoolDom> UpdateAsync(int schoolId, SchoolDom updatedSchool)
        {
            var existingSchool = await GetSchoolById(schoolId);

            if (existingSchool == null)
            {
                throw new SchoolNotFoundException("School not found");
            }

            var existingSchoolDom = _mapper.Map<SchoolDom>(existingSchool);
            if (existingSchoolDom.Equals(updatedSchool))
            {
                return existingSchoolDom;
            }

            // Update only the properties you want to change
            existingSchool.Name = updatedSchool.Name;

            // No need to call Update() explicitly on tracked entities
            await _dbJoggingContext.SaveChangesAsync();

            return _mapper.Map<SchoolDom>(existingSchool);
        }

        public async Task<SchoolDom> UpsertAsync(int? schoolId, SchoolDom updatedSchool)
        {
            SimpleSchool? currentSchool = null;

            if (schoolId.HasValue)
            {
                currentSchool = await GetSchoolById(schoolId.Value);
            }
            else
            {
                currentSchool = await GetSchoolByName(updatedSchool.Name);
            }

            if (currentSchool != null)
            {
                var currentSchoolDom = _mapper.Map<SchoolDom>(currentSchool);
                if (currentSchoolDom.Equals(updatedSchool))
                {
                    return currentSchoolDom;
                }

                // Update the tracked entity directly
                currentSchool.Name = updatedSchool.Name;

                await _dbJoggingContext.SaveChangesAsync();

                return _mapper.Map<SchoolDom>(currentSchool);
            }

            return await AddAsync(updatedSchool);
        }

        public async Task DeleteAsync(int id)
        {
            var school = await GetSchoolById(id);

            if (school == null)
            {
                throw new SchoolNotFoundException("School not found");
            }

            _dbJoggingContext.SimpleSchools.Remove(school);
            await _dbJoggingContext.SaveChangesAsync();
        }

        private async Task<SimpleSchool?> GetSchoolById(int schoolId)
        {
            return await _dbJoggingContext.SimpleSchools
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.SchoolId == schoolId);
        }

        private async Task<SimpleSchool?> GetSchoolByName(string schoolName)
        {
            return await _dbJoggingContext.SimpleSchools
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name == schoolName);
        }
    }
}
