using Contracts;
using UserActivity.Application.DTOs;
using UserActivity.Application.Interfaces;

namespace UserActivity.Application.Services;
public class AuthCountService : IAuthCountService
{
    private readonly ILoginEventRepository _loginEventRepository;
    private readonly IUserInfoService _userInfoService;

    public AuthCountService(ILoginEventRepository loginEventRepository, IUserInfoService userInfoService)
    {
        _loginEventRepository = loginEventRepository;
        _userInfoService = userInfoService;
    }

    public async Task<IEnumerable<UserLoginCountDto>> GetCountAsync(DateTime startDate, DateTime endDate, string userId)
    {
        var loginEvents = await _loginEventRepository.GetLoginCountAsync(startDate, endDate, userId);
        var userLoginCountDtos = new List<UserLoginCountDto>();

        foreach (var group in loginEvents.GroupBy(le => le.UserId))
        {
            var user = await _userInfoService.GetUserByIdAsync(group.Key);

            userLoginCountDtos.Add(new UserLoginCountDto
            {
                UserId = group.Key,
                LoginCount = group.Count(),
                UserName = user.Name,
                Email = user.Email
            });
        }
        return userLoginCountDtos;
    }
}

