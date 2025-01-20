//using Microsoft.IdentityModel.Tokens;
//using System;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using TelefoniaMovilBackend.Models;

//public class JwtTokenFactory : IJwtTokenFactory
//{
//    private readonly IConfiguration _configuration;

//    public JwtTokenFactory(IConfiguration configuration)
//    {
//        _configuration = configuration;
//    }

//    public string GenerateJwtToken(Usuario usuario)
//    {
//        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
//        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

//        var claims = new[]
//        {
//            new Claim(JwtRegisteredClaimNames.Sub, usuario.Email),
//            new Claim("role", usuario.IsAdmin ? "admin" : "user"),
//            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
//            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
//            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
//        };

//        var token = new JwtSecurityToken(
//            issuer: _configuration["Jwt:Issuer"],
//            audience: _configuration["Jwt:Audience"],
//            claims: claims,
//            expires: DateTime.Now.AddMinutes(30),
//            signingCredentials: credentials);

//        return new JwtSecurityTokenHandler().WriteToken(token);
//    }
//}
