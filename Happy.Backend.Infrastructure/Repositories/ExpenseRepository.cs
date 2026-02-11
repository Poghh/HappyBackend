using Happy.Backend.Application.Interfaces;
using Happy.Backend.Domain.Entities;
using Happy.Backend.Infrastructure.Data;

namespace Happy.Backend.Infrastructure.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly HappyDbContext _db;

    public ExpenseRepository(HappyDbContext db)
    {
        _db = db;
    }

    public async Task<Expense?> GetByIdAsync(Guid id)
    {
        return await _db.Expenses.FindAsync(id);
    }

    public async Task AddAsync(Expense entity)
    {
        _db.Expenses.Add(entity);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Expense entity)
    {
        _db.Expenses.Update(entity);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _db.Expenses.FindAsync(id);
        if (entity != null)
        {
            _db.Expenses.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
