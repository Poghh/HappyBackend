using Happy.Backend.Domain.Entities;

namespace Happy.Backend.Application.Interfaces;

public interface IExpenseRepository
{
    Task<Expense?> GetByIdAsync(Guid id);
    Task AddAsync(Expense entity);
    Task UpdateAsync(Expense entity);
    Task DeleteAsync(Guid id);
}
