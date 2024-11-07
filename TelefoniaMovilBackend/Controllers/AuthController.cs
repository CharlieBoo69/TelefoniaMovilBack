using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TelefoniaMovilBackend.Models;
using Microsoft.Extensions.Configuration;
using TelefoniaMovilBackend.Data; // Importa el contexto
using System.Linq;
using System;

namespace TelefoniaMovilBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context; // Agrega el contexto de base de datos

        public AuthController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin userLogin)
        {
            // Busca el usuario en la base de datos
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Email == userLogin.Username && u.Password == userLogin.Password);

            if (usuario == null)
            {
                return Unauthorized("Credenciales incorrectas");
            }

            // Genera el token con el claim "IsAdmin"
            var token = GenerateJwtToken(usuario.IsAdmin);

            return Ok(new { token, role = usuario.IsAdmin ? "admin" : "user" });
        }

        private string GenerateJwtToken(bool isAdmin)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, isAdmin ? "admin" : "user"),
                new Claim("role", isAdmin ? "admin" : "user"),  // Añade el rol al token
                new Claim("IsAdmin", isAdmin.ToString()),       // Añade el claim de IsAdmin
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class UserLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
