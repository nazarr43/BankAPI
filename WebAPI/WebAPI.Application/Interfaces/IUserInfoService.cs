using WebAPI.Domain.Entities;

namespace WebAPI.Application.Interfaces;
public interface IUserInfoService
{
    Task<ApplicationUser> GetUserByIdAsync(string id);
}

