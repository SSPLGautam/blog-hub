using BlogApp.Shared.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthApiController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;

    public AuthApiController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
            return Unauthorized();

        if (!await _userManager.CheckPasswordAsync(user, model.Password))
            return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);

        var token = JwtHelper.GenerateToken(user.Id, user.Email, roles.ToList());

        return Ok(new { token });
    }
}