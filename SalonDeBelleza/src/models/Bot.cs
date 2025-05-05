using System.ComponentModel.DataAnnotations;

namespace SalonDeBelleza.src.models
{
    public class EstadoConversacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TelefonoUsuario { get; set; }

        public PasoConversacion PasoActual { get; set; } = PasoConversacion.Inicio;

        public DateTime Fecha { get; set; }

        public string Servicio { get; set; }

        public int ColaboradorID { get; set; }

        public string Hora { get; set; }
    }

    public enum PasoConversacion
    {
        Inicio,
        Fecha,
        Servicio,
        Colaborador,
        Hora
    }

    public class ConfirmacionCitaRequest
    {
        public int CitaID { get; set; }
        public string Estado {  get; set; }
    }
}
