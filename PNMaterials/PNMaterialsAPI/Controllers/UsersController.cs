using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    private ActionResult? EnsureSuccess(IdentityResult result, string operation)
    {
        if (result.Succeeded)
            return null;

        var errors = result.Errors.ToArray();

        if (errors.Any(e => e.Code is nameof(IdentityErrorDescriber.ConcurrencyFailure)))
            return Problem("Данные пользователя устарели.", statusCode: StatusCodes.Status409Conflict);

        if (errors.Any(e => e.Code is nameof(IdentityErrorDescriber.UserAlreadyInRole)))
            return Problem("У пользователя уже есть такая роль.", statusCode: StatusCodes.Status409Conflict);

        if (errors.Any(e => e.Code is nameof(IdentityErrorDescriber.UserNotInRole)))
            return Problem("У пользователя нет такой роли.", statusCode: StatusCodes.Status409Conflict);

        if (errors.Any(e => e.Code is nameof(IdentityErrorDescriber.DuplicateUserName)))
            return Problem("Этот логин уже занят. Введите другой.", statusCode: StatusCodes.Status409Conflict);

        if (errors.Any(e => e.Code is nameof(IdentityErrorDescriber.InvalidUserName)))
            return Problem("Логин пуст или содержит недопустимые символы.", statusCode: StatusCodes.Status400BadRequest);

        if (errors.Any(e => e.Code is nameof(IdentityErrorDescriber.PasswordTooShort)))
            return Problem($"Пароль должен содержать не менее {_userManager.Options.Password.RequiredLength} символов.", statusCode: StatusCodes.Status400BadRequest);

        var details = string.Join("; ", errors.Select(e => $"{e.Code}: {e.Description}"));

        throw new InvalidOperationException($"{operation}: {details}");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        List<UserDto> users = new List<UserDto>();

        var dbUsers = await _userManager.Users.ToListAsync();

        foreach (var user in dbUsers)
        {
            users.Add(new UserDto 
            {
                Id = user.Id,
                Username = user.UserName!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                Roles = (await _userManager.GetRolesAsync(user)).ToList()
            });
        }

        return users;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null)
            return Problem(detail: "Пользователь не найден.", statusCode: StatusCodes.Status404NotFound);

        return Ok(new UserDto
        {
            Id = user.Id,
            Username = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName,
            Roles = (await _userManager.GetRolesAsync(user)).ToList()
        });
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

        var userResult = EnsureSuccess(await _userManager.CreateAsync(user, dto.Password), "Создание пользователя");

        if (userResult != null)
            return userResult;

        foreach (string role in dto.Roles)
        {
            var roleResult = EnsureSuccess(await _userManager.AddToRoleAsync(user, role), "Добавление роли пользователю");

            if (roleResult != null)
                return roleResult;
        }

        return Ok(new UserDto
        {
            Id = user.Id,
            Username = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName,
            Roles = (await _userManager.GetRolesAsync(user)).ToList()
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

        var userResult = EnsureSuccess(await _userManager.UpdateAsync(user), "Изменение профиля пользователя");

        if (userResult != null)
            return userResult;

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
        var result = EnsureSuccess(await _userManager.ResetPasswordAsync(user, token, dto.NewPassword), "Изменение пароля пользователя");

        if (result != null)
            return result;

        return NoContent();
    }

    [HttpGet("roles")]
    public async Task<ActionResult<List<string>>> GetAllRoles()
    {
        return PNRoles.All.ToList();
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
        var addedRoles = dto.Roles.Except(userRoles).ToArray();
        var removedRoles = userRoles.Except(dto.Roles).ToArray();

        if (removedRoles.Contains(PNRoles.Admin))
        {
            if ((await _userManager.GetUsersInRoleAsync(PNRoles.Admin)).Count() == 1)
                return Problem(detail: $"Нельзя убрать роль {PNRoles.Admin} у последнего администратора.", statusCode: StatusCodes.Status400BadRequest);
        }

        if (addedRoles.Length > 0)
        {
            var addResult = EnsureSuccess(await _userManager.AddToRolesAsync(user, addedRoles), "Добавление новых ролей пользователю");

            if (addResult != null)
                return addResult;
        }

        if (removedRoles.Length > 0)
        {
            var removeResult = EnsureSuccess(await _userManager.RemoveFromRolesAsync(user, removedRoles), "Снятие ролей у пользователя");

            if (removeResult != null)
                return removeResult;
        }

        return Ok(new RolesDto
        {
            Roles = (await _userManager.GetRolesAsync(user)).ToList()
        });
    }

}
