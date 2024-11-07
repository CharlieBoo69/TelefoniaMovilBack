namespace TelefoniaMovilBackend.Models
{
    public class Suscripcion
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int PlanId { get; set; }
        public string NumeroTelefono { get; set; }
        public DateTime FechaSuscripcion { get; set; }
        public Usuario? Usuario { get; set; }
        public Plan? Plan { get; set; }

    }
}
