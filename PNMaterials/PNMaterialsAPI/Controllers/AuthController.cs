using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PNMaterialsAPI.Identity;
using PNMaterialsContracts;
using PNMaterialsInfrastructure;
using PNMaterialsInfrastructure.Identity;

namespace PNMaterialsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private UserManager<PNUser> _userManager { get; }
    private SignInManager<PNUser> _signInManager { get; }
    private TokenService _tokenService { get; }

    public AuthController(AppDbContext db, UserManager<PNUser> userManager, SignInManager<PNUser> signInManager, TokenService tokenService)
    {
        _db = db;
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

        var refreshToken = new RefreshToken
        {
            Token = _tokenService.GenerateRefreshToken(),
            ExpiresOn = DateTime.UtcNow.AddDays(7),
            UserId = user.Id,
        };

        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync();

        return Ok(new LoginResultDto
        {
            Token = issued.Token,
            Expiration = issued.Expiration,
            RefreshToken = refreshToken.Token,
            Login = user.UserName!,
            FullName = $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim(),
            Roles = roles.ToArray()
        });
    }
}
