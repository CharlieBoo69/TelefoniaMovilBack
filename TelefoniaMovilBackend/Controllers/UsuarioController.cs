using Microsoft.AspNetCore.Mvc;
using TelefoniaMovilBackend.Data;
using TelefoniaMovilBackend.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LoginRequest = TelefoniaMovilBackend.Models.LoginRequest;


namespace TelefoniaMovilBackend.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuario
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuario/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Busca el usuario en la base de datos con el correo y la contraseña proporcionados
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);

            if (usuario == null)
            {
                return Unauthorized();
            }

            // Lista de claims, donde incluimos el email y el rol de administrador
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, usuario.Email),
        new Claim("IsAdmin", usuario.IsAdmin.ToString()) // Añadimos el claim de admin
    };

            // Llave de seguridad y credenciales de firma para el token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Your_Secret_Key"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Generamos el token JWT
            var token = new JwtSecurityToken(
                issuer: "your_issuer",
                audience: "your_audience",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            // Retornamos el token en la respuesta
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }


        // POST: api/Usuario
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = usuario.Id }, usuario);
        }

        // PUT: api/Usuario/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Usuario/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }

}
