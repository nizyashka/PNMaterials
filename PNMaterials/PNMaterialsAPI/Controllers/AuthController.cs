using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    [HttpPost("refresh")]
    public async Task<ActionResult<TokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        RefreshToken existing = await _db.RefreshTokens.Include(t => t.User).FirstOrDefaultAsync(t => t.Token == request.RefreshToken);

        if (existing == null)
            return Problem(detail: "Отсутствует токен обновления.", statusCode: StatusCodes.Status401Unauthorized);

        if (existing.ExpiresOn < DateTime.UtcNow)
            return Problem(detail: "Токен обновления просрочен.", statusCode: StatusCodes.Status401Unauthorized);

        var roles = await _userManager.GetRolesAsync(existing.User);
        var accessToken = _tokenService.CreateToken(existing.User, roles);

        var newRefreshToken = new RefreshToken
        {
            Token = _tokenService.GenerateRefreshToken(),
            ExpiresOn = DateTime.UtcNow.AddDays(7),
            UserId = existing.UserId
        };

        _db.RefreshTokens.Add(newRefreshToken);
        _db.RefreshTokens.Remove(existing);
        await _db.SaveChangesAsync();

        return Ok(new TokenResponse { AccessToken = accessToken.Token, RefreshToken = newRefreshToken.Token, AccessTokenExpiresAt = accessToken.Expiration });
    }
}
