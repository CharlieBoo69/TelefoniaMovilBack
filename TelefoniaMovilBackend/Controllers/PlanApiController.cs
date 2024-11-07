using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelefoniaMovilBackend.Data;
using TelefoniaMovilBackend.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TelefoniaMovilBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PlanApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PlanApi
        [HttpGet]
        [AllowAnonymous] // Permite el acceso sin autenticación para ver la lista de planes
        public async Task<ActionResult<IEnumerable<Plan>>> GetPlanes()
        {
            return await _context.Planes.ToListAsync();
        }

        // GET: api/PlanApi/ByOperadora/{operadora}
        [HttpGet("ByOperadora/{operadora}")]
        [AllowAnonymous] // Permite el acceso sin autenticación para ver planes por operadora
        public async Task<ActionResult<IEnumerable<Plan>>> GetPlanesByOperadora(string operadora)
        {
            if (operadora == "Todas las Operadoras")
            {
                return await _context.Planes.ToListAsync();
            }
            return await _context.Planes.Where(p => p.Operadora == operadora).ToListAsync();
        }


        // GET: api/PlanApi/5
        [HttpGet("{id}")]
        [AllowAnonymous] // Permite el acceso sin autenticación para ver detalles de un plan
        public async Task<ActionResult<Plan>> GetPlan(int id)
        {
            var plan = await _context.Planes.FindAsync(id);

            if (plan == null)
            {
                return NotFound();
            }

            return plan;
        }

        // POST: api/PlanApi
        [HttpPost]
        public async Task<ActionResult<Plan>> PostPlan(Plan plan)
        {
            if (!IsAdmin())
            {
                return Forbid("Solo los administradores pueden crear planes");
            }

            _context.Planes.Add(plan);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlan", new { id = plan.Id }, plan);
        }

        // PUT: api/PlanApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlan(int id, Plan plan)
        {
            if (!IsAdmin())
            {
                return Forbid("Solo los administradores pueden editar planes");
            }

            if (id != plan.Id)
            {
                return BadRequest();
            }

            _context.Entry(plan).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlanExists(id))
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

        // DELETE: api/PlanApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlan(int id)
        {
            if (!IsAdmin())
            {
                return Forbid("Solo los administradores pueden eliminar planes");
            }

            var plan = await _context.Planes.FindAsync(id);
            if (plan == null)
            {
                return NotFound();
            }

            _context.Planes.Remove(plan);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlanExists(int id)
        {
            return _context.Planes.Any(e => e.Id == id);
        }

        // Método auxiliar para verificar si el usuario es administrador
        private bool IsAdmin()
        {
            var isAdminClaim = User.Claims.FirstOrDefault(c => c.Type == "IsAdmin");
            return isAdminClaim != null && bool.TryParse(isAdminClaim.Value, out bool isAdmin) && isAdmin;
        }
    }
}
