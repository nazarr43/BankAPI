using Contracts;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using UserActivity.Application.Configuration;
using UserActivity.Application.Interfaces;

namespace UserActivity.Application.Services;
public class UserInfoService : IUserInfoService
{
    private const string _apiPath = "api/webapi/UserInfo/";
    private readonly HttpClient _httpClient;

    public UserInfoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<UserInfoDto> GetUserByIdAsync(string userId)
    {
        var url = $"{_apiPath}{userId}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<UserInfoDto>(content);
    }
}

