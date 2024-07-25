using WebAPI.Domain.Entities;

namespace WebAPI.Application.Interfaces;
public interface IUserInfoRepository
{
    Task<ApplicationUser> GetByIdAsync(string id);
}

