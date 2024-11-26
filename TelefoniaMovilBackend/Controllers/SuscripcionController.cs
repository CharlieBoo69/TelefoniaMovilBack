using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TelefoniaMovilBackend.Data;
using TelefoniaMovilBackend.Models;
using System.IdentityModel.Tokens.Jwt;



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

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] NewSubscriptionRequest request)
        {
            try
            {
                // Obtener el UserId desde la cookie o sesión
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return Unauthorized("Usuario no autenticado.");
                }

                // Validar que el número de teléfono no esté ya registrado
                var existingSubscription = await _context.Suscripciones
                    .FirstOrDefaultAsync(s => s.NumeroTelefono == request.NumeroTelefono);
                if (existingSubscription != null)
                {
                    return BadRequest("El número de teléfono ya tiene una suscripción.");
                }

                // Crear una nueva suscripción
                var nuevaSuscripcion = new Suscripcion
                {
                    UsuarioId = userId.Value,
                    PlanId = request.PlanId,
                    NumeroTelefono = request.NumeroTelefono,
                    FechaSuscripcion = DateTime.Now
                };

                _context.Suscripciones.Add(nuevaSuscripcion);
                await _context.SaveChangesAsync();

                return Ok("Suscripción creada con éxito.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear la suscripción: {ex.Message}");
            }
        }

        // Clase para manejar la solicitud de nueva suscripción
        public class NewSubscriptionRequest
        {
            public int PlanId { get; set; }
            public string NumeroTelefono { get; set; }
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


        [HttpGet("User")]
        public async Task<IActionResult> GetSuscripcionesByUser()
        {
            try
            {
                // Recuperar el UserId desde la sesión
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    Console.WriteLine("No estás autenticado. La sesión no contiene un UserId.");
                    return Unauthorized("No estás autenticado.");
                }

                Console.WriteLine($"Usuario autenticado con ID: {userId}");

                // Recuperar las suscripciones asociadas al usuario
                var suscripciones = await _context.Suscripciones
                    .Where(s => s.UsuarioId == userId)
                    .Include(s => s.Plan) // Incluye datos del plan relacionado
                    .Select(s => new
                    {
                        s.Id,
                        s.NumeroTelefono,
                        s.FechaSuscripcion,
                        PlanNombre = s.Plan.Nombre
                    })
                    .ToListAsync();

                Console.WriteLine($"Se encontraron {suscripciones.Count} suscripciones para el usuario con ID: {userId}");

                return Ok(suscripciones);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetSuscripcionesByUser: {ex.Message}");
                return StatusCode(500, "Ocurrió un error interno en el servidor.");
            }
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSuscripcion(int id)
        {
            var suscripcion = await _context.Suscripciones.FindAsync(id);

            if (suscripcion == null)
            {
                return NotFound("La suscripción no existe.");
            }

            // Si el usuario es administrador, permite la eliminación
            if (IsAdmin())
            {
                _context.Suscripciones.Remove(suscripcion);
                await _context.SaveChangesAsync();
                return NoContent();
            }

            // Validar que el usuario autenticado es el propietario de la suscripción
            int authenticatedUserId = GetAuthenticatedUserId();
            if (suscripcion.UsuarioId != authenticatedUserId)
            {
                return Forbid("No tienes permiso para eliminar esta suscripción.");
            }

            // Eliminar la suscripción
            _context.Suscripciones.Remove(suscripcion);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        private int GetAuthenticatedUserId()
        {
            // Verifica si la cookie existe
            if (!Request.Cookies.ContainsKey("UserId"))
            {
                throw new UnauthorizedAccessException("No se encontró la cookie de UserId.");
            }

            // Obtiene el valor del UserId desde la cookie
            string userId = Request.Cookies["UserId"];

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("La cookie de UserId está vacía.");
            }

            return int.Parse(userId); // Convierte el valor a un entero
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
