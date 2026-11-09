using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PNMaterialsContracts;
using PNMaterialsInfrastructure.Identity;

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
            var usernameTaken = userResult.Errors.Any(error => error.Code is nameof(IdentityErrorDescriber.DuplicateUserName));
            var shortPassword = userResult.Errors.Any(error => error.Code is nameof(IdentityErrorDescriber.PasswordTooShort));
            var status = usernameTaken ? StatusCodes.Status409Conflict : StatusCodes.Status400BadRequest;
            var message = usernameTaken ? "Этот логин уже занят. Введите другой." 
                : (shortPassword ? $"Пароль должен содержать не менее {_userManager.Options.Password.RequiredLength} символов." : "Ошибка создания пользователя.");
            message += (usernameTaken && shortPassword) ? $" Пароль должен содержать не менее {_userManager.Options.Password.RequiredLength} символов." : "";

            return Problem(detail: message, statusCode: status);
        }

        foreach (string role in dto.Roles)
        {
            var roleResult = await _userManager.AddToRoleAsync(user, role);

            if (!roleResult.Succeeded)
            {
                var hasConflict = roleResult.Errors.Any(e => e.Code is nameof(IdentityErrorDescriber.ConcurrencyFailure));
                var status = hasConflict ? StatusCodes.Status409Conflict : StatusCodes.Status400BadRequest;
                var message = hasConflict ? "Пользователь создан. При назначении роли его данные изменились. Обновите карточку и проверьте назначенные роли."
                    : $"Пользователь создан, но назначить роль {role} не удалось. Откройте карточку пользователя и проверьте его роли";

                return Problem(detail: message, statusCode: status);
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
            var usernameTaken = userResult.Errors.Any(error => error.Code is nameof(IdentityErrorDescriber.DuplicateUserName));

            var status = hasConflict ? StatusCodes.Status409Conflict : StatusCodes.Status400BadRequest;
            var message = hasConflict ? (usernameTaken ? "Этот логин уже занят. Введите другой." : "Пользователь был изменен. Обновите карточку и попробуйте снова.")
                : "Ошибка изменения профиля пользователя.";

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

    [HttpPut("{id:int}/password")]
    public async Task<ActionResult> EditPassword(int id, PasswordEditDto dto)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null)
            return Problem(detail: "Пользователь не найден.", statusCode: StatusCodes.Status404NotFound);

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

        if (!result.Succeeded)
        {
            var hasConflict = result.Errors.Any(error => error.Code is nameof(IdentityErrorDescriber.ConcurrencyFailure));
            var shortPassword = result.Errors.Any(error => error.Code is nameof(IdentityErrorDescriber.PasswordTooShort));
            var status = hasConflict ? StatusCodes.Status409Conflict : StatusCodes.Status400BadRequest;
            var message = hasConflict ? "Пользователь был изменен. Обновите карточку и попробуйте снова." 
                : (shortPassword ? $"Пароль должен содержать не менее {_userManager.Options.Password.RequiredLength} символов." : "Ошибка изменения пароля.");

            return Problem(detail: message, statusCode: status);
        }

        return NoContent();
    }

    [HttpPut("{id:int}/roles")]
    public async Task<ActionResult<RolesDto>> EditRoles(int id, RolesDto dto)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null)
            return Problem(detail: "Пользователь не найден.", statusCode: StatusCodes.Status404NotFound);

        if (!dto.Roles.All(x => PNRoles.All.Contains(x)))
            return Problem(detail: "Попытка добавить пользователю несуществующую роль.", statusCode: StatusCodes.Status400BadRequest);

        var userRoles = await _userManager.GetRolesAsync(user);
        var addedRoles = dto.Roles.Except(userRoles);
        var removedRoles = userRoles.Except(dto.Roles);

        IdentityResult addResult = await _userManager.AddToRolesAsync(user, addedRoles);

        if (!addResult.Succeeded)
        {
            var hasConflict = addResult.Errors.Any(error => error.Code is nameof(IdentityErrorDescriber.ConcurrencyFailure)
            or nameof(IdentityErrorDescriber.UserAlreadyInRole));
            var status = hasConflict ? StatusCodes.Status409Conflict : StatusCodes.Status400BadRequest;
            var message = hasConflict ? "Пользователь был изменен. Обновите карточку и попробуйте снова." : "Не удалось назначить выбранные роли.";

            return Problem(detail: message, statusCode: status);
        }

        IdentityResult removeResult = await _userManager.RemoveFromRolesAsync(user, removedRoles);

        if (!removeResult.Succeeded)
        {
            var hasConflict = removeResult.Errors.Any(error => error.Code is nameof(IdentityErrorDescriber.ConcurrencyFailure)
            or nameof(IdentityErrorDescriber.UserNotInRole));
            var status = hasConflict ? StatusCodes.Status409Conflict : StatusCodes.Status400BadRequest;
            var message = hasConflict ? "Пользователь был изменен. Обновите карточку и попробуйте снова." : "Не удалось снять выбранные роли.";

            return Problem(detail: message, statusCode: status);
        }

        return Ok(dto);
    }
}
