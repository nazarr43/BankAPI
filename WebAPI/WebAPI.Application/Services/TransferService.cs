using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Threading.Tasks;
using WebAPI.Application.DTOs;
using WebAPI.Application.Interfaces;
using WebAPI.Application.Model;
using WebAPI.Domain.Constants;
using WebAPI.Domain.Entities;

namespace WebAPI.Application.Services
{
    public class TransferService : ITransferService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrencyApiClient _currencyService;
        private static readonly object _lock = new object();

        public TransferService(IUnitOfWork unitOfWork, ICurrencyApiClient currencyService)
        {
            _unitOfWork = unitOfWork;
            _currencyService = currencyService;
        }

        public async Task<Result<bool>> TransferAsync(TransferRequest request)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var accountRepository = _unitOfWork.Repository<Account>();
                var transferRepository = _unitOfWork.Repository<Transaction>();

                var fromAccount = await accountRepository.GetByIdAsync(request.FromAccountId);
                var toAccount = await accountRepository.GetByIdAsync(request.ToAccountId);

                if (fromAccount == null || toAccount == null)
                {
                    Log.Warning("Transfer failed: One or both accounts not found. FromAccountId: {FromAccountId}, ToAccountId: {ToAccountId}", request.FromAccountId, request.ToAccountId);
                    return Result<bool>.Failure("One or both accounts not found");
                }

                if (fromAccount.Balance < request.Amount)
                {
                    Log.Warning("Transfer failed: Insufficient funds. FromAccountId: {FromAccountId}, Balance: {Balance}, Amount: {Amount}", request.FromAccountId, fromAccount.Balance, request.Amount);
                    return Result<bool>.Failure("Insufficient funds");
                }

                if (fromAccount.Currency != request.Currency || toAccount.Currency != request.Currency)
                {
                    request.Amount = await _currencyService.ConvertCurrencyAsync(fromAccount.Currency, request.Currency, request.Amount);
                }
                lock (_lock)
                {
                    fromAccount.Balance -= request.Amount;
                    toAccount.Balance += request.Amount;
                    
                }
                await accountRepository.UpdateAsync(fromAccount);
                await accountRepository.UpdateAsync(toAccount);
                var transactionEntity = new TransferTransaction
                {
                    Id = Guid.NewGuid(),
                    FromAccountId = request.FromAccountId,
                    ToAccountId = request.ToAccountId,
                    Amount = request.Amount,
                    Timestamp = DateTime.UtcNow,
                    UserId = request.UserId,
                    PaymentPurpose = request.PaymentPurpose,
                    TransactionType = TransactionTypes.Transfer
                };

                await transferRepository.AddAsync(transactionEntity);

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitAsync(transaction);
                Log.Information("Transfer successful: FromAccountId: {FromAccountId}, ToAccountId: {ToAccountId}, Amount: {Amount}, UserId: {UserId}", request.FromAccountId, request.ToAccountId, request.Amount, request.UserId);

                return Result<bool>.Success(true);
            }
            catch
            {
                await _unitOfWork.RollbackAsync(transaction);
                Log.Error("An error occurred during the transfer operation. FromAccountId: {FromAccountId}, ToAccountId: {ToAccountId}, Amount: {Amount}, UserId: {UserId}", request.FromAccountId, request.ToAccountId, request.Amount, request.UserId);
                return Result<bool>.Failure("An error occurred during the transfer operation");
            }

        }
    }
}
