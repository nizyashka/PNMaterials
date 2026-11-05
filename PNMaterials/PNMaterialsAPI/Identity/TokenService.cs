using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using PNMaterialsInfrastructure.Identity;
using System.Security.Claims;
using System.Text;

namespace PNMaterialsAPI.Identity;

public class TokenService(IOptions<JwtOptions> options)
{
    public record Token(string TokenString, DateTimeOffset Expiration);

    public Token CreateToken(PNUser user, IEnumerable<string>? roles = null)
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

        DateTimeOffset dateTimeOffset = DateTime.UtcNow.Add(options.Value.AccessTokenLifetime);

        var securityTokenDescriptor = new SecurityTokenDescriptor 
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = options.Value.Issuer,
            Audience = options.Value.Audience,
            Expires = dateTimeOffset.UtcDateTime,
            SigningCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256)
        };

        var token = new JsonWebTokenHandler().CreateToken(securityTokenDescriptor);

        return new Token(token, dateTimeOffset);
    }
}
