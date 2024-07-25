using Contracts;

namespace UserActivity.Application.Interfaces;
public interface IUserInfoService
{
    Task<UserInfoDto> GetUserByIdAsync(string userId);
}

