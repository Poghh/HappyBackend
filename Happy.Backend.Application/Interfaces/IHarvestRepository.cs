using Happy.Backend.Domain.Entities;

namespace Happy.Backend.Application.Interfaces;

public interface IHarvestRepository
{
    Task<Harvest?> GetByIdAsync(Guid id);
    Task AddAsync(Harvest entity);
    Task UpdateAsync(Harvest entity);
    Task DeleteAsync(Guid id);
}
