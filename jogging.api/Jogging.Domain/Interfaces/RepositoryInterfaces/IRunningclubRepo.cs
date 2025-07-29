using Jogging.Persistence.Models;

namespace Jogging.Domain.Interfaces.RepositoryInterfaces;

public interface IRunningclubRepo
{
    public Task<List<Runningclub>> GetAllAsync();
    public Task<Runningclub> GetByIdAsync(int id);
    public Task<Runningclub> GetByNameAsync(string name);
    public Task<Runningclub> GetByUserIdAsync(int userId);
    public Task AddAsync(Runningclub runningclub);
    public Task UpdateAsync(int runningId, Runningclub runningclub);
    public Task DeleteAsync(int id);

    Task<List<RunningClubWithData>> GetAllWithDataAsync();
    Task RegisterPersonToClubAsync(int userId, int clubId);
}