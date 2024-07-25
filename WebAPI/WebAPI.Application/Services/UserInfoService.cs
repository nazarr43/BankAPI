using Contracts;
using WebAPI.Application.Interfaces;
using WebAPI.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace WebAPI.Application.Services
{
    public class UserInfoService : IUserInfoService
    {
        private const string CacheKeyPrefix = "UserInfo:";
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly ICachingService _cachingService;
        private readonly ILogger<UserInfoService> _logger; 

        public UserInfoService(IUserInfoRepository userInfoRepository, ICachingService cachingService, ILogger<UserInfoService> logger)
        {
            _userInfoRepository = userInfoRepository;
            _cachingService = cachingService;
            _logger = logger;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            var cacheKey = $"{CacheKeyPrefix}{id}";

            var cachedUserDto = await _cachingService.GetCacheAsync<ApplicationUser>(cacheKey).ConfigureAwait(false);
            if (cachedUserDto != null)
            {
                return cachedUserDto;
            }

            var user = await _userInfoRepository.GetByIdAsync(id);
            if (user != null)
            {
                await _cachingService.SetCacheAsync(cacheKey, user, TimeSpan.FromHours(1));
                _logger.LogInformation("Data retrieved from repository and cached for user ID: {UserId}", id);
            }
            return user;
        }
    }
}
