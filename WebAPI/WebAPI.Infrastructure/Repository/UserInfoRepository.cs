using Microsoft.EntityFrameworkCore;
using WebAPI.Application.Interfaces;
using WebAPI.Domain.Entities;
using WebAPI.Infrastructure.Data;

namespace WebAPI.Infrastructure.Repository;
public class UserInfoRepository : IUserInfoRepository
{
    private readonly AppDbContext _context;

    public UserInfoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationUser> GetByIdAsync(string id)
    {
        return await _context.Users.FindAsync(id);
    }
}

