using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using PNMaterialsInfrastructure.Identity;
using System.Security.Claims;

namespace PNMaterialsAPI.Middleware;

public class UserBlockMiddleware
{
    private readonly RequestDelegate _next;

    public UserBlockMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserManager<PNUser> userManager)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }

        var user = await userManager.GetUserAsync(context.User);

        if (user == null)
        {
            await context.ChallengeAsync();
            return;
        }

        if (user.IsBlocked)
        {
            await Results.Problem(detail: "Пользователь заблокирован.", statusCode: StatusCodes.Status403Forbidden).ExecuteAsync(context);
            return;
        }

        if (context.User.Identity is not ClaimsIdentity identity)
        {
            await context.ChallengeAsync();
            return;
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        var oldRoleClaims = identity.FindAll(identity.RoleClaimType).ToArray();

        foreach (var claim in oldRoleClaims)
        {
            identity.RemoveClaim(claim);
        }

        foreach (var role in currentRoles)
        {
            identity.AddClaim(new Claim(identity.RoleClaimType, role));
        }

        await _next(context);
    }
}
