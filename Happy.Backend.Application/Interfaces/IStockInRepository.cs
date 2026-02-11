using Happy.Backend.Domain.Entities;

namespace Happy.Backend.Application.Interfaces;

public interface IStockInRepository
{
    Task<StockIn?> GetByProductCodeAndPhoneAsync(string productCode, string phone);
    Task AddAsync(StockIn entity);
    Task UpdateAsync(StockIn entity);
    Task DeleteByProductCodeAndPhoneAsync(string productCode, string phone);
}
