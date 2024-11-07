namespace TelefoniaMovilBackend.Models
{
    public class Plan
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Costo { get; set; }
        public int Datos { get; set; }
        public int Minutos { get; set; }
        public int SMS { get; set; }
        public string Operadora { get; set; }
        public string? BeneficiosAdicionales { get; set; } // Hacer que sea opcional

    }
}
