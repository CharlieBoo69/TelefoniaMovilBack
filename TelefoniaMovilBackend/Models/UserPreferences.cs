namespace TelefoniaMovilBackend.Models
{
    public class UserPreferences
    {
        public decimal Costo { get; set; } // Preferencia de costo
        public int Datos { get; set; } // Preferencia de datos en GB
        public int Minutos { get; set; } // Preferencia de minutos
        public int SMS { get; set; } // Preferencia de SMS
        public string? Operadora { get; set; } // Preferencia opcional de operadora (puede ser null o vacío si no especifica)
        public double PesoCosto { get; set; } // Peso para el costo
        public double PesoDatos { get; set; } // Peso para los datos
        public double PesoMinutos { get; set; } // Peso para los minutos
        public double PesoSMS { get; set; } // Peso para los SMS

    }


}
