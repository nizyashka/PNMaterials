using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PNMaterialsContracts;
using PNMaterialsInfrastructure;
using PNMaterialsInfrastructure.Identity;
using System.Net.NetworkInformation;
using static Azure.Core.HttpHeader;

namespace PNMaterialsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = PNRoles.Admin)]
public class UsersController : ControllerBase
{
    private readonly UserManager<PNUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public UsersController(UserManager<PNUser> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto dto)
    {
        if (dto.Roles.Count() <= 0)
            return Problem(detail: "Для создаваемого пользователя не выбраны роли.", statusCode: StatusCodes.Status400BadRequest);

        if (!dto.Roles.All(x => PNRoles.All.Contains(x)))
            return Problem(detail: "Попытка добавить пользователю несуществующую роль.", statusCode: StatusCodes.Status400BadRequest);

        PNUser user = new PNUser
        {
            UserName = dto.Username.Trim(),
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            MiddleName = dto.MiddleName?.Trim()
        };
        var userResult = await _userManager.CreateAsync(user, dto.Password);

        if (!userResult.Succeeded)
        {
            var usernameTaken = userResult.Errors.Any(e => e.Code == "DuplicateUserName");
            var status = usernameTaken ? StatusCodes.Status409Conflict : StatusCodes.Status400BadRequest;
            var message = string.Join(Environment.NewLine, userResult.Errors.Select(e => e.Description));
            return Problem(detail: message, statusCode: status);
        }

        foreach (string role in dto.Roles)
        {
            var roleResult = await _userManager.AddToRoleAsync(user, role);

            if (!roleResult.Succeeded)
            {
                var message = string.Join(Environment.NewLine, roleResult.Errors.Select(e => e.Description));
                return Problem(detail: message, statusCode: StatusCodes.Status400BadRequest);
            }
        }

        return Ok(new UserDto
        {
            Id = user.Id,
            Username = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName,
            Roles = (await _userManager.GetRolesAsync(user)).ToArray()
        });
    }

    [HttpPut("{id:int}/profile")]
    public async Task<ActionResult<ProfileDto>> EditProfile(int id, ProfileEditDto dto)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null)
            return Problem(detail: "Пользователь не найден.", statusCode: StatusCodes.Status404NotFound);

        user.UserName = dto.Username.Trim();
        user.FirstName = dto.FirstName.Trim();
        user.LastName = dto.LastName.Trim();
        user.MiddleName = dto.MiddleName?.Trim();

        var userResult = await _userManager.UpdateAsync(user);

        if (!userResult.Succeeded)
        {
            var hasConflict = userResult.Errors.Any(error =>
                error.Code is nameof(IdentityErrorDescriber.DuplicateUserName)
                           or nameof(IdentityErrorDescriber.ConcurrencyFailure));

            var status = hasConflict ? StatusCodes.Status409Conflict : StatusCodes.Status400BadRequest;

            var message = string.Join(Environment.NewLine, userResult.Errors.Select(error => error.Description));
            return Problem(detail: message, statusCode: status);
        }

        return Ok(new ProfileDto 
        {
            Username = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName
        });
    }
}
