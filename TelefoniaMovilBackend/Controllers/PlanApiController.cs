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

        //private readonly IPlanRepository _planRepository;

        //public PlanApiController(IPlanRepository planRepository)
        //{
        //    _planRepository = planRepository;
        //}



        [HttpPost("GetRecommendedPlans")]
        public IActionResult GetRecommendedPlans([FromBody] UserPreferences preferences)
        {
            try
            {
                // Obtener todos los planes disponibles de la base de datos
                var plans = _context.Planes.ToList();

                // Validar si hay planes disponibles
                if (!plans.Any())
                {
                    return NotFound("No hay planes disponibles en este momento.");
                }

                // Registrar la cantidad de planes obtenidos para debug
                Console.WriteLine($"Total de planes disponibles: {plans.Count}");

                // Calcular las recomendaciones
                var recommendedPlans = CalculateRecommendations(plans, preferences);

                // Devolver las recomendaciones al frontend
                return Ok(recommendedPlans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error al procesar la solicitud: {ex.Message}");
            }
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

        private double CalculateSimilarity(Plan plan, UserPreferences preferences)
        {
            // similitud de costo datos minutos SMS
            double costSimilarity = 1 - (Math.Abs((double)preferences.Costo - (double)plan.Costo) / Math.Max((double)preferences.Costo, (double)plan.Costo));
            double datosSimilarity = 1 - (Math.Abs(preferences.Datos - plan.Datos) / (double)Math.Max(preferences.Datos, plan.Datos));
            double minutosSimilarity = 1 - (Math.Abs(preferences.Minutos - plan.Minutos) / (double)Math.Max(preferences.Minutos, plan.Minutos));
            double smsSimilarity = 1 - (Math.Abs(preferences.SMS - plan.SMS) / (double)Math.Max(preferences.SMS, plan.SMS));

            // preferencia de operadora
            double operadoraSimilarity = 0; // no afecta si no se especifica
            if (!string.IsNullOrEmpty(preferences.Operadora))
            {
                operadoraSimilarity = plan.Operadora == preferences.Operadora ? 1 : 0;
            }

            // Promediar las similitudes
            int divisor = string.IsNullOrEmpty(preferences.Operadora) ? 4 : 5;
            return ((costSimilarity + datosSimilarity + minutosSimilarity + smsSimilarity + operadoraSimilarity) / divisor) * 100;
        }

        private List<object> CalculateRecommendations(List<Plan> plans, UserPreferences preferences)
        {
            // Normalización de los pesos
            double totalPeso = preferences.PesoCosto + preferences.PesoDatos + preferences.PesoMinutos + preferences.PesoSMS;
            if (totalPeso > 0)
            {
                preferences.PesoCosto /= totalPeso;
                preferences.PesoDatos /= totalPeso;
                preferences.PesoMinutos /= totalPeso;
                preferences.PesoSMS /= totalPeso;
            }

            // Normalización de los datos**
            var normalizedPlans = plans.Select(plan => new
            {
                Plan = plan,
                NormalizedCost = 1 - ((double)plan.Costo / (double)plans.Max(p => p.Costo)),
                NormalizedDatos = (double)plan.Datos / plans.Max(p => p.Datos),
                NormalizedMinutos = (double)plan.Minutos / plans.Max(p => p.Minutos),
                NormalizedSMS = (double)plan.SMS / plans.Max(p => p.SMS)
            }).ToList();

            // score similarity y hybridScore
            var scoredPlans = normalizedPlans.Select(nPlan => new
            {
                Plan = nPlan.Plan,
                Score = (preferences.PesoCosto * nPlan.NormalizedCost) +
                        (preferences.PesoDatos * nPlan.NormalizedDatos) +
                        (preferences.PesoMinutos * nPlan.NormalizedMinutos) +
                        (preferences.PesoSMS * nPlan.NormalizedSMS),
                Similarity = CalculateSimilarity(nPlan.Plan, preferences), //  calcular similitud
                HybridScore = 0.5 * (
                    (preferences.PesoCosto * nPlan.NormalizedCost) +
                    (preferences.PesoDatos * nPlan.NormalizedDatos) +
                    (preferences.PesoMinutos * nPlan.NormalizedMinutos) +
                    (preferences.PesoSMS * nPlan.NormalizedSMS)
                ) + (0.5 * CalculateSimilarity(nPlan.Plan, preferences)) // score y similarity
            })
            .OrderByDescending(sp => sp.HybridScore) // Ordenar por HybridScore
            .ToList();

            Console.WriteLine($"Planes evaluados:");
            foreach (var sp in scoredPlans)
            {
                Console.WriteLine($"Plan ID: {sp.Plan.Id}, Nombre: {sp.Plan.Nombre}, Score: {sp.Score}, Similarity: {sp.Similarity}, HybridScore: {sp.HybridScore}");
            }

            //  tres mejores opciones
            return scoredPlans.Take(3).Select(sp => new
            {
                Plan = sp.Plan,
                HybridScore = sp.HybridScore,
                SimilarityPercentage = sp.Similarity
            }).ToList<object>();
        }




    }
}
