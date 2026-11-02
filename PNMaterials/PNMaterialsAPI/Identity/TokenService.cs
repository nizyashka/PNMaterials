using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using PNMaterialsInfrastructure.Identity;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PNMaterialsAPI.Identity;

public class TokenService(IOptions<JwtOptions> options)
{
    public record Token(string TokenString, DateTimeOffset Expiration);

    public enum TokenType { Access, Refresh }

    public Token CreateToken(TokenType tokenType, PNUser user, IEnumerable<string>? roles = null)
    {
        var claims = new List<Claim> 
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
        };

        if (roles != null && roles.Any())
        {
            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role));
            }
        }

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey));

        var lifetime = tokenType == TokenType.Access ? options.Value.AccessTokenLifetime : options.Value.RefreshTokenLifetime;
        DateTimeOffset dateTimeOffset = DateTime.UtcNow.Add(lifetime);


        var securityTokenDescriptor = new SecurityTokenDescriptor 
        {
            Subject = new ClaimsIdentity(claims),
            TokenType = tokenType.ToString(),
            Issuer = options.Value.Issuer,
            Audience = options.Value.Audience,
            Expires = dateTimeOffset.UtcDateTime,
            SigningCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256)
        };

        var token = new JsonWebTokenHandler().CreateToken(securityTokenDescriptor);

        return new Token(token, dateTimeOffset);
    }

    public async Task<int?> ValidateRefreshTokenAsync(string refreshToken)
    {
        var validationParameters = new TokenValidationParameters
        {
            RequireSignedTokens = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey)),
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],

            ValidateIssuer = true,
            ValidIssuer = options.Value.Issuer,

            ValidateAudience = true,
            RequireAudience = true,
            ValidAudience = options.Value.Audience,

            ValidTypes = [TokenType.Refresh.ToString()],

            ValidateLifetime = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero
        };

        var result = await new JsonWebTokenHandler().ValidateTokenAsync(refreshToken, validationParameters);

        if (!result.IsValid)
            return null;

        var subject = result.ClaimsIdentity
            .FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (!int.TryParse(subject, out var userId) || userId <= 0)
            return null;

        return userId;
    }
}
