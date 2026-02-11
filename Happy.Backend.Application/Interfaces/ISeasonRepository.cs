using Happy.Backend.Domain.Entities;

namespace Happy.Backend.Application.Interfaces;

public interface ISeasonRepository
{
    Task<Season?> GetByIdAsync(Guid id);
    Task AddAsync(Season entity);
    Task UpdateAsync(Season entity);
    Task DeleteAsync(Guid id);
}
