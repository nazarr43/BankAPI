using WebAPI.Domain.Entities;

namespace WebAPI.Application.Interfaces;
public interface ITransactionStatisticsService
{
    Task<int> GetTransactionCountByUserIdAsync(string userId);
    Task<decimal> GetVarianceByUserIdAsync(string userId);
    Task<IEnumerable<DepositTransaction>> GetTransactionsByTypeAsync(string transactionType, int page, int pageSize);
    Task<(decimal Q1, decimal Q2, decimal Q3)> GetTransactionQuartilesAsync();
    Task UpsertTransactionAsync(Transaction transaction);
}

