using Microsoft.AspNetCore.Mvc;
using Prometheus;
using Serilog;
using System.Diagnostics.Metrics;
using System.Security.Claims;
using System.Text;
using UserActivity.Application.DTOs;
using UserActivity.Application.Interfaces;

namespace UserActivity.Presentation.Controllers;
[Route("api/useractivity/[controller]")]
[ApiController]
public class AuthCountController : ControllerBase
{
    private readonly IAuthCountService _authCountService;

    public AuthCountController(IAuthCountService authCountService)
    {
        _authCountService = authCountService;
    }

    [HttpGet("logins/count")]
    public async Task<IActionResult> GetLoginCount([FromQuery] DateRangeDto dateRange, [FromQuery] string userId)
    {
        var userLoginCounts = await _authCountService.GetCountAsync(dateRange.StartDate, dateRange.EndDate, userId);
        Log.Information("Returning login count: UserId={UserId}, Count={Count}", userId, userLoginCounts);
        return Ok(userLoginCounts);
    }
}

