using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PNMaterialsInfrastructure.Identity;
using PNMaterialsContracts;
using PNMaterialsAPI.Identity;

namespace PNMaterialsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private UserManager<PNUser> _userManager { get; }
    private SignInManager<PNUser> _signInManager { get; }
    private TokenService _tokenService { get; }

    public AuthController(UserManager<PNUser> userManager, SignInManager<PNUser> signInManager, TokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResultDto>> Login(LoginDto dto)
    {
        PNUser? user = await _userManager.FindByNameAsync(dto.Login);

        if (user is null)
            return Problem(detail: "Неверный логин или пароль.", statusCode: StatusCodes.Status401Unauthorized);

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);

        if (signInResult.IsLockedOut)
            return Problem(detail: "Пользователь заблокирован.", statusCode: StatusCodes.Status403Forbidden);

        if (!signInResult.Succeeded)
            return Problem(detail: "Неверный логин или пароль.", statusCode: StatusCodes.Status401Unauthorized);

        var roles = await _userManager.GetRolesAsync(user);
        var issued = _tokenService.CreateToken(user, roles);

        return Ok(new LoginResultDto
        {
            Token = issued.Token,
            Expiration = issued.Expiration,
            Login = user.UserName!,
            FullName = $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim(),
            Roles = roles.ToArray()
        });
    }
}
