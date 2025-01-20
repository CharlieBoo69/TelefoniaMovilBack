//using TelefoniaMovilBackend.Data;
//using TelefoniaMovilBackend.Models;
//using System.Linq;

//public class AuthService : IAuthService
//{
//    private readonly ApplicationDbContext _context;

//    public AuthService(ApplicationDbContext context)
//    {
//        _context = context;
//    }

//    public Usuario Authenticate(string username, string password)
//    {
//        // Busca el usuario en la base de datos
//        var usuario = _context.Usuarios
//            .FirstOrDefault(u => u.Email == username && u.Password == password);

//        if (usuario == null)
//        {
//            throw new UnauthorizedAccessException("Credenciales incorrectas");
//        }

//        return usuario;
//    }

//    public void Logout()
//    {
//        // La lógica del logout puede manejarse aquí si es necesario
//        // Por ahora, dejamos este método vacío como ejemplo.
//    }
//}
