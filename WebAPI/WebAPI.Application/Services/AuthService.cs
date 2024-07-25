using WebAPI.Application.DTOs;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using WebAPI.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using WebAPI.Application.Services;
using AutoMapper;
using WebAPI.Domain.Entities;
using WebAPI.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.Application.Configuration;
using Contracts;
using Serilog;
namespace WebAPI.Application.Services;
public class AuthService : IAuthService
{
    private readonly IUserManagerDecorator<ApplicationUser> _userManager;
    private readonly ISignInManagerDecorator<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly IKafkaEventService _kafkaEventService;
    public AuthService(
        IUserManagerDecorator<ApplicationUser> userManager,
        ISignInManagerDecorator<ApplicationUser> signInManager,
        ITokenService tokenService,
        IMapper mapper,
        IKafkaEventService kafkaEventService
       )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _mapper = mapper;
        _kafkaEventService = kafkaEventService;
    }
    public async Task<string> LoginAsync(LoginRequest request)
    {
        var result = await _signInManager.PasswordSignInAsync(request.Username, request.Password, false, false);

        if (!result.Succeeded)
        {
            Log.Warning("Login failed for user: {Username}", request.Username);
            return null;
        }
        var user = await _userManager.FindByNameAsync(request.Username);
        var loginEvent = new LoginEvent { UserId = user.Id.ToString(), Timestamp = DateTime.UtcNow };
        await _kafkaEventService.PublishLoginEventAsync(loginEvent);

        var token = await _tokenService.GenerateJwtToken(user);
        Log.Information("User {Username} logged in successfully and JWT token generated", request.Username);

        return token;
    }
    public async Task<object> RegisterAsync(RegisterRequest request)
    {
        var user = _mapper.Map<ApplicationUser>(request);
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return null;
        }
        if (!string.IsNullOrEmpty(request.Role))
        {
            var roles = request.Role.Split(',');
            foreach (var role in roles)
            {
                await _userManager.AddToRoleAsync(user, role.Trim());
            }
        }
        return user.Id;
    }
}