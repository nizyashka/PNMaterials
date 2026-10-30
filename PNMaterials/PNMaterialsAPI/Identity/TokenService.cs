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
    public (string Token, DateTimeOffset Expiration) CreateToken(PNUser user, IEnumerable<string> roles)
    {
        var claims = new List<Claim> 
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim("role", role));
        }

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey));
        DateTimeOffset dateTimeOffset = DateTime.UtcNow.AddHours(options.Value.ExpirationInHours);


        var securityTokenDescriptor = new SecurityTokenDescriptor 
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = options.Value.Issuer,
            Audience = options.Value.Audience,
            Expires = dateTimeOffset.UtcDateTime,
            SigningCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256)
        };

        var token = new JsonWebTokenHandler().CreateToken(securityTokenDescriptor);

        return (token, dateTimeOffset);
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }
}
