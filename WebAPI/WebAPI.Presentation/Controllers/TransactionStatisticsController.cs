using Microsoft.AspNetCore.Mvc;
using WebAPI.Application.Interfaces;
using WebAPI.Domain.Entities;

namespace WebAPI.Presentation.Controllers;
[Route("api/webapi/[controller]")]
[ApiController]
public class TransactionStatisticsController : ControllerBase
{
    private readonly ITransactionStatisticsService _transactionStatisticsService;

    public TransactionStatisticsController(ITransactionStatisticsService transactionStatisticsService)
    {
        _transactionStatisticsService = transactionStatisticsService;
    }

    [HttpGet("count/{userId}")]
    public async Task<IActionResult> GetTransactionCountByUserId(string userId)
    {
        var count = await _transactionStatisticsService.GetTransactionCountByUserIdAsync(userId);
        return Ok(count);
    }

    [HttpGet("variance/{userId}")]
    public async Task<IActionResult> GetVarianceByUserIdAsync(string userId)
    {
        var variance = await _transactionStatisticsService.GetVarianceByUserIdAsync(userId);
        return Ok(variance);
    }

    [HttpGet("transactionsByType")]
    public async Task<ActionResult<IEnumerable<DepositTransaction>>> GetTransactionsByType(string transactionType, int page = 1, int pageSize = 10)
    {
        var transactions = await _transactionStatisticsService.GetTransactionsByTypeAsync(transactionType, page, pageSize);
        return Ok(transactions);
    }

    [HttpGet("quartiles")]
    public async Task<IActionResult> GetTransactionQuartilesAsync()
    {
        var (Q1, Q2, Q3) = await _transactionStatisticsService.GetTransactionQuartilesAsync();
        var quartiles = new
        {
            Q1,
            Q2,
            Q3
        };
        return Ok(quartiles);
    }

    [HttpPost("upsert")]
    public async Task<IActionResult> UpsertTransactionAsync(Transaction transaction)
    {
        await _transactionStatisticsService.UpsertTransactionAsync(transaction);
        return Ok("Transaction upserted successfully.");
    }

}

