using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Infrastructure.Data;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.Uow;
using WebAPI.Application.Interfaces;

namespace WebAPI.Presentation.Controllers
{
    [Route("api/webapi/[controller]")]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        private readonly ILogger<UserInfoController> _logger;
        private readonly IUserInfoService _userInfoService;

        public UserInfoController(IUserInfoService userInfoService, ILogger<UserInfoController> logger)
        {
            _userInfoService = userInfoService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userInfoService.GetUserByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User with ID {Id} not found", id);
                return NotFound();
            }
            return Ok(new { Username = user.UserName, Email = user.Email });
        }
    }
}
