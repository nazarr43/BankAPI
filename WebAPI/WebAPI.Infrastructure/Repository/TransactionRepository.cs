using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Resources;
using WebAPI.Application.Interfaces;
using WebAPI.Domain.Entities;
using WebAPI.Infrastructure.Data;

namespace WebAPI.Infrastructure.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        private DbConnection GetConnection()
        {
            return _context.Database.GetDbConnection();
        }

        public async Task<int> GetTransactionCountByUserIdAsync(string userId)
        {
            using var connection = GetConnection();
            return await connection.QuerySingleAsync<int>(SqlQueries.GetTransactionCountByUserId, new { UserId = userId });
        }

        public async Task<decimal> GetVarianceByUserIdAsync(string userId)
        {
            using var connection = GetConnection();
            return await connection.QuerySingleAsync<decimal>(SqlQueries.GetVarianceByUserId, new { UserId = userId });
        }

        public async Task<IEnumerable<DepositTransaction>> GetTransactionsByTypeAsync(string transactionType, int page = 1, int pageSize = 10)
        {
            int offset = (page - 1) * pageSize;
            using var connection = GetConnection();
            var transactions = await connection.QueryAsync<DepositTransaction>(SqlQueries.GetTransactionsByType, new { TransactionType = transactionType, PageSize = pageSize, Offset = offset });
            return transactions.AsList();
        }

        public async Task<(decimal Q1, decimal Q2, decimal Q3)> GetTransactionQuartilesAsync()
        {
            using var connection = GetConnection();
            var parameters = new
            {
                Percentile1 = 0.25,
                Percentile2 = 0.5,
                Percentile3 = 0.75
            };
            return await connection.QueryFirstOrDefaultAsync<(decimal, decimal, decimal)>(SqlQueries.GetTransactionQuartiles, parameters);
        }

        public async Task UpsertTransactionAsync(Transaction transaction)
        {
            using var connection = GetConnection();
            await connection.ExecuteAsync(SqlQueries.UpsertTransaction, transaction);
        }
    }
}
