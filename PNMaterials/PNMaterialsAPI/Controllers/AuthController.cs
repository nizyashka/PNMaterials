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
        var accessToken = _tokenService.CreateToken(TokenService.TokenType.Access, user, roles);
        var refreshToken = _tokenService.CreateToken(TokenService.TokenType.Refresh, user);


        return Ok(new LoginResultDto
        {
            Token = accessToken.TokenString,
            Expiration = accessToken.Expiration,
            RefreshToken = refreshToken.TokenString,
            Login = user.UserName!,
            FullName = $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim(),
            Roles = roles.ToArray()
        });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<TokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        int? userId = await _tokenService.ValidateRefreshTokenAsync(request.RefreshToken);

        if (userId == null)
            return Problem(detail: "Не удалось подтвердить валидность токена обновления.", statusCode: StatusCodes.Status401Unauthorized);

        PNUser? user = await _userManager.FindByIdAsync(userId.Value.ToString());

        if (user == null)
            return Problem(detail: "Пользователь не найден.", statusCode: StatusCodes.Status401Unauthorized);

        if (await _userManager.IsLockedOutAsync(user))
            return Problem(detail: "Пользователь заблокирован.", statusCode: StatusCodes.Status403Forbidden);

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.CreateToken(TokenService.TokenType.Access, user, roles);
        var refreshToken = _tokenService.CreateToken(TokenService.TokenType.Refresh, user);

        return Ok(new TokenResponse { AccessToken = accessToken.TokenString, RefreshToken = refreshToken.TokenString, AccessTokenExpiresAt = accessToken.Expiration });
    }
}
