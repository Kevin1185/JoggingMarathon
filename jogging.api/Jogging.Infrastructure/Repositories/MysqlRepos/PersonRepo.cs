using AutoMapper;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Helpers;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
using Jogging.Infrastructure.Models.SearchModels.Person;
using Microsoft.EntityFrameworkCore;

using Jogging.Persistence.Models.Address;
using Jogging.Persistence.Models.School;
using System.Runtime.InteropServices;
using Jogging.Persistence.Models.Person;
using Jogging.Persistence.Context;


namespace Jogging.Infrastructure.Repositories.MysqlRepos;

public class PersonRepo : IPersonRepo
{
    private readonly JoggingContext _dbJoggingContext;
    private readonly IGenericRepo<AddressDom> _addressRepo;
    private readonly IGenericRepo<SchoolDom> _schoolRepo;
    private readonly IMapper _mapper;

    public PersonRepo(JoggingContext joggingContext, IGenericRepo<AddressDom> addressRepo, IGenericRepo<SchoolDom> schoolRepo, IMapper mapper)
    {
        _dbJoggingContext = joggingContext;
        _addressRepo = addressRepo;
        _schoolRepo = schoolRepo;
        _mapper = mapper;
    }

    public async Task<List<PersonDom>> GetAllAsync()
    {
        var persons = await _dbJoggingContext.People.ToListAsync();

        if (persons.Count <= 0)
        {
            throw new PersonException("No persons found");
        }

        return _mapper.Map<List<PersonDom>>(persons);
    }


    //TODO: check this!!!
    public async Task<List<PersonDom>> GetBySearchValueAsync(string searchValue)
    {
        var persons = await _dbJoggingContext.People
            .FromSqlInterpolated($"CALL get_persons_by_search_value({searchValue})")
            .ToListAsync();

        if (persons == null || persons.Count == 0)
        {
            throw new PersonNotFoundException("Person not found");
        }

        return _mapper.Map<List<PersonDom>>(persons);
    }

    public async Task<PersonDom> GetByIdAsync(int personId)
    {
        var person = await GetPersonById(personId);

        if (person == null)
        {
            throw new PersonException("No person found");
        }

        return _mapper.Map<PersonDom>(person);

    }

    public async Task<PersonDom> GetByEmailAsync(string email)
    {
        var person = await _dbJoggingContext.People.Include(p => p.Profile).FirstOrDefaultAsync(p => p.Email == email);

        if (person == null)
        {
            throw new PersonException("No person found");
        }

        return _mapper.Map<PersonDom>(person);
    }


    Task<PersonDom> IGenericRepo<PersonDom>.UpdateAsync(int id, PersonDom updatedItem)
    {
        throw new NotImplementedException();
    }

    public async Task<PersonDom> AddAsync(PersonDom newPersonDom)
    {

        var address = await _addressRepo.UpsertAsync(null, newPersonDom.Address);

        newPersonDom.AddressId = address.Id;
        newPersonDom.Address = address;

        if (newPersonDom.School != null)
        {
            SchoolDom? school = await _schoolRepo.UpsertAsync(null, newPersonDom.School);
            newPersonDom.SchoolId = school.Id;
            newPersonDom.School = school;
        }

        var personToAdd = _mapper.Map<ExtendedPerson>(newPersonDom);

        var addedPerson = _dbJoggingContext.People.Add(personToAdd); 
        await _dbJoggingContext.SaveChangesAsync();

        if(addedPerson.Entity == null)
        {
            throw new PersonException("Something went wrong while adding your account");
        }
        newPersonDom.Id = personToAdd.Id;

        return _mapper.Map<PersonDom>(newPersonDom);
    }

    public async Task<(PersonDom, PersonDom, bool shouldSendEmail)> UpdateAsync(int personId, PersonDom updatedPerson)
    {
        var currentPerson = await GetPersonById(personId);

        if (currentPerson == null)
        {
            throw new PersonException("Person not found");
        }

        var originalPerson = _mapper.Map<ExtendedPerson>(currentPerson);

        var isAddressUpdated = await UpdateAddressIfNeeded(currentPerson, updatedPerson.Address);
        var isSchoolUpdated = await UpdateSchoolIfNeeded(currentPerson, updatedPerson.School);
        bool shouldSendEmail = Convert.ToChar(currentPerson.Gender) != updatedPerson.Gender || currentPerson.BirthDate != updatedPerson.BirthDate;

        if (isAddressUpdated || isSchoolUpdated || !_mapper.Map<PersonDom>(currentPerson).Equals(updatedPerson))
        {
            currentPerson.LastName = updatedPerson.LastName;
            currentPerson.FirstName = updatedPerson.FirstName;
            currentPerson.Ibannumber = updatedPerson.IBANNumber;
            currentPerson.BirthDate = updatedPerson.BirthDate;
            currentPerson.Gender = updatedPerson.Gender.ToString();

            _dbJoggingContext.People.Update(currentPerson);
            await _dbJoggingContext.SaveChangesAsync();
        }

        return (_mapper.Map<PersonDom>(originalPerson), _mapper.Map<PersonDom>(currentPerson), shouldSendEmail);
    }

    public async Task UpdatePersonEmailAsync(int personId, PersonDom updatedPerson)
    {
        var currentPerson = await GetPersonById(personId);

        if (currentPerson == null)
        {
            throw new PersonException("Person not found");
        }

        if(updatedPerson.UserId == null)
        {
            throw new PersonException("User Id is required");
        }

        currentPerson.Email = updatedPerson.Email;
        currentPerson.UserId = Guid.Parse(updatedPerson.UserId);

        var person = _dbJoggingContext.People.Update(currentPerson);
        await _dbJoggingContext.SaveChangesAsync();

        if (person.Entity == null)
        {
            throw new PersonException("Something went wrong while updating your account");
        }
    }

    public Task<PersonDom> UpsertAsync(int? addressId, PersonDom updatedItem)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(int personId)
    {
        var person = await GetPersonById(personId);

        if (person == null)
        {
            throw new PersonNotFoundException("This person doesn't exist anymore");
        }

        _dbJoggingContext.People.Remove(person);
        await _dbJoggingContext.SaveChangesAsync();
    }

    public async Task<PersonDom?> GetByGuidAsync(string userId)
    {
        try
        {
            var userIdGuid = Guid.Parse(userId);
            var person = await _dbJoggingContext.AuthUsers
            .Where(au => au.Id == userIdGuid)
            .SingleOrDefaultAsync();

            if (person == null)
            {
                throw new PersonException("Something went wrong while getting your account information");
            }

            var extendedPerson = await GetByEmailAsync(person.Username);

            return _mapper.Map<PersonDom>(extendedPerson);
        }
        catch (Exception)
        {
            throw new PersonException("Something went wrong while getting your account information");
        }        
    }


    private async Task<ExtendedPerson?> GetPersonById(int personId)
    {
        return await
            _dbJoggingContext.People
            .Where(p => p.Id == personId)
            .Include(p => p.Address)
            .Include(p => p.School)
            .SingleOrDefaultAsync();
    }

    private async Task<bool> UpdateAddressIfNeeded(ExtendedPerson person, AddressDom updatedAddress)
    {
        var currentAddress = _mapper.Map<AddressDom>(person.Address);

        if (!currentAddress.Equals(updatedAddress))
        {
            var address = await _addressRepo.UpsertAsync(null, updatedAddress);
            person.AddressId = address.Id;
            person.Address = _mapper.Map<ExtendedAddress>(address);
            return true;
        }

        return false;
    }

    private async Task<bool> UpdateSchoolIfNeeded(ExtendedPerson person, SchoolDom? updatedPersonSchool)
    {
        //var currentSchool = await _dbJoggingContext.Schools.FindAsync(person.SchoolId);
        var currentSchool = _mapper.Map<SchoolDom>(person.School);

        if (currentSchool != null && updatedPersonSchool == null)
        {
            person.SchoolId = null;
            person.School = null;
            return true;
        }

        if ((currentSchool == null && updatedPersonSchool != null) ||
            (updatedPersonSchool != null && currentSchool != null && !currentSchool.Equals(updatedPersonSchool)))
        {
            var school = await _schoolRepo.UpsertAsync(null, updatedPersonSchool);

            await _dbJoggingContext.SaveChangesAsync();
            person.SchoolId = school.Id;
            person.School = _mapper.Map<ExtendedSchool>(school);

            return true;
        }

        return false;
    }
}