using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using WebAPI.Application.DTOs;
using WebAPI.Application.Interfaces;
using WebAPI.Application.Services;
using WebAPI.Domain.Entities;
using Xunit;

namespace WebAPI.Tests
{
    public class TransferServiceTests
    {
        [Fact]
        public async Task TransferAsync_TransferSuccessful_ReturnsSuccessResult()
        {
            var transferRequest = new TransferRequest
            {
                FromAccountId = Guid.NewGuid(),
                ToAccountId = Guid.NewGuid(),
                Amount = 100m,
                Currency = "USD",
                UserId = "user123",
                PaymentPurpose = "Test transfer"
            };

            var fromAccount = new Account
            {
                Id = transferRequest.FromAccountId,
                Balance = 500m
            };

            var toAccount = new Account
            {
                Id = transferRequest.ToAccountId,
                Balance = 200m
            };

            var accountRepositoryMock = new Mock<IRepository<Account>>();
            accountRepositoryMock.Setup(repo => repo.GetByIdAsync(transferRequest.FromAccountId)).ReturnsAsync(fromAccount);
            accountRepositoryMock.Setup(repo => repo.GetByIdAsync(transferRequest.ToAccountId)).ReturnsAsync(toAccount);

            var currencyApiClientMock = new Mock<ICurrencyApiClient>();
            currencyApiClientMock.Setup(x => x.ConvertCurrencyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>()))
                                 .ReturnsAsync((string fromCurrency, string toCurrency, decimal amount) => amount);

            var transferRepositoryMock = new Mock<IRepository<Transaction>>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var transferService = new TransferService(unitOfWorkMock.Object, currencyApiClientMock.Object);

            var result = await transferService.TransferAsync(transferRequest);

            result.IsSuccess.Should().BeFalse();
        }
    }
}