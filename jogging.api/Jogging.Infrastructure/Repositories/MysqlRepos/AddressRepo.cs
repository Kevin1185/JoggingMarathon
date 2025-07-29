using AutoMapper;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
using Jogging.Persistence.Context;
using Jogging.Persistence.Models.Address;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;


namespace Jogging.Infrastructure.Repositories.MysqlRepos;

public class AddressRepo : IGenericRepo<AddressDom>
{
    private readonly JoggingContext _dbJoggingContext;
    private readonly IMapper _mapper;

    public AddressRepo(JoggingContext joggingContext, IMapper mapper)
    {
        _dbJoggingContext = joggingContext;
        _mapper = mapper;
    }

    public async Task<List<AddressDom>> GetAllAsync()
    {
        var addresses = await _dbJoggingContext.SimpleAddresses.ToListAsync();

        if (addresses.Count <= 0)
        {
            throw new AddressException("No addresses found");
        }

        return _mapper.Map<List<AddressDom>>(addresses);
    }

    public async Task<AddressDom> GetByIdAsync(int id)
    {
        var address = await GetSimpleAddressByIdAsync(id);

        if (address == null)
        {
            throw new AddressException("No address found");
        }

        return _mapper.Map<AddressDom>(address);
    }

    public async Task<AddressDom> AddAsync(AddressDom addressDom)
    {
        var address = new SimpleAddress {
            Street = addressDom.Street,
            HouseNumber = addressDom.HouseNumber,
            City = addressDom.City,
            ZipCode = addressDom.ZipCode
        };

        await _dbJoggingContext.SimpleAddresses.AddAsync(address);
        await _dbJoggingContext.SaveChangesAsync();

        return _mapper.Map<AddressDom>(address);
    }

    public async Task<AddressDom> UpdateAsync(int id, AddressDom addressDom)
    {
        var currentAddress = await GetSimpleAddressByIdAsync(id);

        if (currentAddress == null)
        {
            throw new AddressException("No address found");
        }

        currentAddress.Street = addressDom.Street;
        currentAddress.HouseNumber = addressDom.HouseNumber;
        currentAddress.City = addressDom.City;
        currentAddress.ZipCode = addressDom.ZipCode;

        _dbJoggingContext.SimpleAddresses.Update(currentAddress);
        await _dbJoggingContext.SaveChangesAsync();

        return _mapper.Map<AddressDom>(currentAddress);
    }

    public async Task DeleteAsync(int id)
    {
        var address = await _dbJoggingContext.SimpleAddresses.FirstOrDefaultAsync(a => a.AddressId == id);

        if (address == null)
        {
            throw new AddressException("No address found");
        }

        _dbJoggingContext.SimpleAddresses.Remove(address);
        await _dbJoggingContext.SaveChangesAsync();
    }

    public async Task<AddressDom> UpsertAsync(int? addressId, AddressDom updatedAddressDom)
    {

        var updatedAddress = new SimpleAddress {
            Street = updatedAddressDom.Street,
            HouseNumber = updatedAddressDom.HouseNumber,
            City = updatedAddressDom.City,
            ZipCode = updatedAddressDom.ZipCode
        };

        SimpleAddress? currentAddress;
        if (addressId.HasValue)
        {
            currentAddress = await GetSimpleAddressByIdAsync(addressId.Value);
        }
        else
        {
            currentAddress = await FindAddressByValues(updatedAddress);
        }

        if (currentAddress == null)
        {
            return await AddAsync(updatedAddressDom);
        }

        if (currentAddress.Equals(updatedAddress))
        {
            return _mapper.Map<AddressDom>(currentAddress);           
        }

        currentAddress.Street = updatedAddress.Street;
        currentAddress.HouseNumber = updatedAddress.HouseNumber;
        currentAddress.City = updatedAddress.City;
        currentAddress.ZipCode = updatedAddress.ZipCode;

        _dbJoggingContext.SimpleAddresses.Update(currentAddress);

        await _dbJoggingContext.SaveChangesAsync();

        return _mapper.Map<AddressDom>(currentAddress);
    }

    private async Task<SimpleAddress> GetSimpleAddressByIdAsync(int addressId)
    {
        var address = await _dbJoggingContext.SimpleAddresses.FirstOrDefaultAsync(a => a.AddressId == addressId);
        if(address == null)
        {
            throw new AddressException("No address found");
        }

        return address;

    }

    private async Task<SimpleAddress> FindAddressByValues(SimpleAddress addressToFind)
    {

        var foundAddress = await _dbJoggingContext.SimpleAddresses
            .Where(a => a.Street == addressToFind.Street &&
            a.HouseNumber == addressToFind.HouseNumber &&
            a.City == addressToFind.City &&
            a.ZipCode == addressToFind.ZipCode)
            .OfType<SimpleAddress>()
            .FirstOrDefaultAsync();

        return foundAddress;
    }
}