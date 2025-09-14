using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Cuest.Models.AppUsers;
using Cuest.Dtos.Auth;

namespace Cuest.Controllers.Auth
{

    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _users;
        private readonly SignInManager<AppUser> _signIn;
        private readonly ITokenService _tokens;

        public AuthController(UserManager<AppUser> users, SignInManager<AppUser> signIn, ITokenService tokens)
        {
            _users = users;
            _signIn = signIn;
            _tokens = tokens;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Register(RegisterDto dto)
        {
            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                DisplayName = dto.DisplayName
            };

            var result = await _users.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            // Optional: add default role
            // await _users.AddToRoleAsync(user, "User");

            return await _tokens.CreateAuthResponseAsync(user);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login(LoginDto dto)
        {
            var user = await _users.FindByEmailAsync(dto.Email);
            if (user is null) return Unauthorized("Invalid credentials.");

            var result = await _signIn.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
            if (!result.Succeeded) return Unauthorized("Invalid credentials.");

            return await _tokens.CreateAuthResponseAsync(user);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDto>> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var user = await _users.FindByIdAsync(userId);
            if (user is null) return Unauthorized();

            return new UserDto(user.Id, user.Email ?? "", user.DisplayName);
        }
    }
}
