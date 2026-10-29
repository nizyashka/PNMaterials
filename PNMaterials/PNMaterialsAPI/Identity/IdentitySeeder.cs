using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PNMaterialsContracts;
using PNMaterialsInfrastructure.Identity;

namespace PNMaterialsAPI.Identity;

public class IdentitySeeder(
    RoleManager<IdentityRole<int>> roleManager,
    UserManager<PNUser> userManager,
    IConfiguration configuration,
    ILogger<IdentitySeeder> logger)
{
    public async Task SeedAsync()
    {
        foreach (var role in PNRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
                EnsureSuccess(await roleManager.CreateAsync(new IdentityRole<int>(role)), $"Создание роли {role}");
        }

        if ((await userManager.GetUsersInRoleAsync(PNRoles.Admin)).Count > 0)
            return;

        var admin = configuration.GetSection("PNAdmin").Get<PNAdminOptions>();
        if (admin is null || string.IsNullOrWhiteSpace(admin.Username) || string.IsNullOrWhiteSpace(admin.Password))
            throw new InvalidOperationException("В системе нет администратора, а в конфигурации не задан раздел PNAdmin с логином и паролем.");

        if (await userManager.FindByNameAsync(admin.Username) is not null)
            throw new InvalidOperationException($"В системе нет администратора, а логин {admin.Username} из раздела PNAdmin уже занят другим пользователем.");

        var user = new PNUser
        {
            UserName = admin.Username,
            FirstName = admin.FirstName,
            LastName = admin.LastName
        };

        EnsureSuccess(await userManager.CreateAsync(user, admin.Password), "Создание нового администратора.");
        EnsureSuccess(await userManager.AddToRoleAsync(user, PNRoles.Admin), "Назначение роли администратора");

        logger.LogInformation("Создан первый администратор {UserName}", user.UserName);
    }

    private static void EnsureSuccess(IdentityResult identityResult, string operation)
    {
        if (!identityResult.Succeeded)
            throw new InvalidOperationException($"{operation}: {string.Join("; ", identityResult.Errors.Select(e => e.Description))}");
    }
}
