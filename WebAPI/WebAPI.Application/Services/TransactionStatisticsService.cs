using WebAPI.Application.Interfaces;
using WebAPI.Domain.Entities;

namespace WebAPI.Application.Services;
public class TransactionStatisticsService : ITransactionStatisticsService
{
    private readonly ITransactionRepository _transactionRepository;

    public TransactionStatisticsService(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }
    public Task<int> GetTransactionCountByUserIdAsync(string userId)
    {
        return _transactionRepository.GetTransactionCountByUserIdAsync(userId);
    }

    public Task<decimal> GetVarianceByUserIdAsync(string userId)
    {
        return _transactionRepository.GetVarianceByUserIdAsync(userId);
    }

    public Task<IEnumerable<DepositTransaction>> GetTransactionsByTypeAsync(string transactionType, int page, int pageSize)
    {
        return _transactionRepository.GetTransactionsByTypeAsync(transactionType, page, pageSize);
    }

    public Task<(decimal Q1, decimal Q2, decimal Q3)> GetTransactionQuartilesAsync()
    {
        return _transactionRepository.GetTransactionQuartilesAsync();
    }

    public Task UpsertTransactionAsync(Transaction transaction)
    {
        return _transactionRepository.UpsertTransactionAsync(transaction);
    }

}

