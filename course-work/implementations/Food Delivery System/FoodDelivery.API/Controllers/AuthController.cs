using FoodDelivery.API.Helpers;
using FoodDelivery.Core.DTOs;
using FoodDelivery.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtHelper _jwtHelper;

        public AuthController(IUserService userService, JwtHelper jwtHelper)
        {
            _userService = userService;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
        {
            if (await _userService.EmailExistsAsync(dto.Email))
                throw new ArgumentException("Имейлът вече се използва.");

            if (await _userService.UsernameExistsAsync(dto.Username))
                throw new ArgumentException("Потребителското име вече се използва.");

            var user = await _userService.CreateUserAsync(dto);

            return Ok(new AuthResponseDto
            {
                Token = _jwtHelper.GenerateToken(user),
                Username = user.Username,
                Role = user.Role,
                UserId = user.Id
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            var user = await _userService.GetByEmailAsync(dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Невалиден имейл или парола.");

            return Ok(new AuthResponseDto
            {
                Token = _jwtHelper.GenerateToken(user),
                Username = user.Username,
                Role = user.Role,
                UserId = user.Id
            });
        }
    }
}