using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TelefoniaMovilBackend.Models;
using Microsoft.Extensions.Configuration;
using TelefoniaMovilBackend.Data; 
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

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Limpia sesión del usuario
            HttpContext.Session.Clear();

            // invalidar las cookies si es necesario
            Response.Cookies.Delete(".AspNetCore.Session");

            // respuesta sesión fue cerrada
            return Ok(new { message = "Sesión cerrada con éxito" });
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin userLogin)
        {
            // busca el usuario en la base de datos
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Email == userLogin.Username && u.Password == userLogin.Password);

            if (usuario == null)
            {
                return Unauthorized("Credenciales incorrectas");
            }

            // almacena el ID del usuario en la sesión
            HttpContext.Session.SetInt32("UserId", usuario.Id);

            // genera el token con los claims 
            var token = GenerateJwtToken(usuario);

            // establece una cookie con el UserId
            Response.Cookies.Append("UserId", usuario.Id.ToString(), new CookieOptions
            {
                HttpOnly = true,        // Proteger contra accesos desde JavaScript
                Secure = true,          // Solo para HTTPS en producción(TRue)
                SameSite = SameSiteMode.Strict// Restricción del uso en el mismo dominio ...SameSite = SameSiteMode.Strict....SameSite = SameSiteMode.None
            });

            return Ok(new { token, role = usuario.IsAdmin ? "admin" : "user" });
        }


        private string GenerateJwtToken(Usuario usuario)
        {
            // clave de seguridad para el token
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // claims del token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Email),              // Email como claim principal (sub)
                new Claim("role", usuario.IsAdmin ? "admin" : "user"),              // Rol del usuario
                new Claim("IsAdmin", usuario.IsAdmin.ToString()),                   // Indica si es administrador
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),        // ID del usuario
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),            // Email explícitamente
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())   // ID único para el token
            };

            // Crear el token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],   // Emisor del token
                audience: _configuration["Jwt:Audience"], // Audiencia del token
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),    // Tiempo de expiración
                signingCredentials: credentials);

            // Generar el token como cadena
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Método auxiliar para verificar si el usuario es administrador
        private bool IsAdmin()
        {
            var isAdminClaim = User.Claims.FirstOrDefault(c => c.Type == "IsAdmin");
            return isAdminClaim != null && bool.TryParse(isAdminClaim.Value, out bool isAdmin) && isAdmin;
        }

        [HttpGet("CheckSession")]
        public IActionResult CheckSession()
        {
            // Recupera el UserId desde la sesión
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized("No hay ningún usuario autenticado.");
            }

            return Ok($"Usuario autenticado con ID: {userId}");
        }
    }

    public class UserLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
