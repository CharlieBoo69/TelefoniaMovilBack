using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TelefoniaMovilBackend.Data;
using TelefoniaMovilBackend.Models;

namespace TelefoniaMovilBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuscripcionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SuscripcionController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/Suscripcion
        [HttpGet]
        public async Task<IActionResult> GetAllSuscripciones()
        {
            if (!IsAdmin())
            {
                return Forbid("Acceso solo para administradores");
            }

            var suscripciones = await _context.Suscripciones.ToListAsync();
            return Ok(suscripciones);
        }


        // GET: api/Suscripcion/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSuscripcion(int id)
        {
            if (!IsAdmin())
            {
                return Forbid("Acceso solo para administradores");
            }

            var suscripcion = await _context.Suscripciones.FindAsync(id);
            if (suscripcion == null)
            {
                return NotFound();
            }
            return Ok(suscripcion);
        }

        // GET: api/Suscripcion/Plan/{planId}
        [HttpGet("Plan/{planId}")]
        public async Task<IActionResult> GetSuscripcionesByPlanId(int planId)
        {
            if (!IsAdmin())
            {
                return Forbid("Acceso solo para administradores");
            }

            var suscripciones = await _context.Suscripciones
                .Where(s => s.PlanId == planId)
                .ToListAsync();

            if (suscripciones == null || suscripciones.Count == 0)
            {
                return NotFound("No se encontraron suscripciones para este plan");
            }

            return Ok(suscripciones);
        }





        // GET: api/Suscripcion/TopPlanes
        [HttpGet("TopPlanes")]
        public async Task<IActionResult> GetTopPlanes()
        {
            if (!IsAdmin())
            {
                return Forbid("Acceso solo para administradores");
            }

            var topPlanes = await _context.Suscripciones
                .GroupBy(s => s.PlanId)
                .Select(group => new
                {
                    PlanId = group.Key,
                    TotalSuscripciones = group.Count()
                })
                .OrderByDescending(s => s.TotalSuscripciones)
                .ToListAsync();

            return Ok(topPlanes);
        }

        // POST: api/Suscripcion
        [HttpPost]
        public async Task<IActionResult> CreateSuscripcion(Suscripcion suscripcion)
        {
            if (!IsAdmin())
            {
                return Forbid("Acceso solo para administradores");
            }

            // Verificar que el número de teléfono sea único
            var existingSuscripcion = await _context.Suscripciones
                .FirstOrDefaultAsync(s => s.NumeroTelefono == suscripcion.NumeroTelefono);

            if (existingSuscripcion != null)
            {
                return BadRequest("El número de teléfono ya está asociado a otra suscripción");
            }

            // Verificar que el usuario y el plan existan
            var usuarioExists = await _context.Usuarios.AnyAsync(u => u.Id == suscripcion.UsuarioId);
            var planExists = await _context.Planes.AnyAsync(p => p.Id == suscripcion.PlanId);

            if (!usuarioExists || !planExists)
            {
                return BadRequest("Usuario o plan no encontrado");
            }

            _context.Suscripciones.Add(suscripcion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateSuscripcion), new { id = suscripcion.Id }, suscripcion);
        }

        // PUT: api/Suscripcion/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSuscripcion(int id, Suscripcion suscripcion)
        {
            if (!IsAdmin())
            {
                return Forbid("Acceso solo para administradores");
            }

            if (id != suscripcion.Id)
            {
                return BadRequest();
            }

            _context.Entry(suscripcion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SuscripcionExists(id))
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

        // DELETE: api/Suscripcion/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSuscripcion(int id)
        {
            if (!IsAdmin())
            {
                return Forbid("Acceso solo para administradores");
            }

            var suscripcion = await _context.Suscripciones.FindAsync(id);
            if (suscripcion == null)
            {
                return NotFound();
            }

            _context.Suscripciones.Remove(suscripcion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SuscripcionExists(int id)
        {
            return _context.Suscripciones.Any(e => e.Id == id);
        }

        // Método auxiliar para verificar si el usuario es administrador
        private bool IsAdmin()
        {
            var isAdminClaim = User.Claims.FirstOrDefault(c => c.Type == "IsAdmin");
            return isAdminClaim != null && bool.TryParse(isAdminClaim.Value, out bool isAdmin) && isAdmin;
        }
    }
}
