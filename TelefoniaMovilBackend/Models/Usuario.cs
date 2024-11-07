namespace TelefoniaMovilBackend.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public List<Suscripcion> Suscripciones { get; set; } = new List<Suscripcion>();

    }
}
