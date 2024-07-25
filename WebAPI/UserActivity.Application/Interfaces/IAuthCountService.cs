using Contracts;
using UserActivity.Application.DTOs;

namespace UserActivity.Application.Interfaces;
public interface IAuthCountService
{
    Task<IEnumerable<UserLoginCountDto>> GetCountAsync(DateTime startDate, DateTime endDate, string userId);
}

