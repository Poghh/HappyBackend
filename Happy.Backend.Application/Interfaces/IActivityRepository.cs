using Happy.Backend.Domain.Entities;

namespace Happy.Backend.Application.Interfaces;

public interface IActivityRepository
{
    Task<Activity?> GetByIdAsync(Guid id);
    Task AddAsync(Activity entity);
    Task UpdateAsync(Activity entity);
    Task DeleteAsync(Guid id);
}
