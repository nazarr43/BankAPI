using Contracts;
using UserActivity.Application.DTOs;

namespace UserActivity.Application.Interfaces;
public interface ILoginEventRepository
{
    Task<IEnumerable<LoginEvent>> GetLoginCountAsync(DateTime startDate, DateTime endDate, string userId);
    Task LogLoginEvent(LoginEvent loginEvent);
}

