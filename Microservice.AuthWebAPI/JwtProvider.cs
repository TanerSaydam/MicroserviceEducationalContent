using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Microservice.AuthWebAPI;

public sealed class JwtProvider
{
    public string CreateToken()
    {
        string issuer = "Issuer";
        string audience = "Audience";
        string secretKey = "my secret keymy secret keymy secret keymy secret keymy secret keymy secret keymy secret keymy secret keymy secret keymy secret keymy secret key";
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

        var claims = new List<Claim>()
        {
            new Claim("UserType","registered"),
        };

        JwtSecurityToken securityToken = new(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: signingCredentials);

        var handler = new JwtSecurityTokenHandler();
        var token = handler.WriteToken(securityToken);

        return token;
    }
}