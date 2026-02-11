using Happy.Backend.Application.Interfaces;
using Happy.Backend.Domain.Entities;
using Happy.Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Happy.Backend.Infrastructure.Repositories;

public class StockInRepository : IStockInRepository
{
    private readonly HappyDbContext _db;

    public StockInRepository(HappyDbContext db)
    {
        _db = db;
    }

    public async Task<StockIn?> GetByProductCodeAndPhoneAsync(string productCode, string phone)
    {
        return await _db.StockIns
            .FirstOrDefaultAsync(x => x.ProductCode == productCode && x.Phone == phone);
    }

    public async Task AddAsync(StockIn entity)
    {
        _db.StockIns.Add(entity);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(StockIn entity)
    {
        _db.StockIns.Update(entity);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteByProductCodeAndPhoneAsync(string productCode, string phone)
    {
        var entity = await _db.StockIns
            .FirstOrDefaultAsync(x => x.ProductCode == productCode && x.Phone == phone);
        if (entity != null)
        {
            _db.StockIns.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
