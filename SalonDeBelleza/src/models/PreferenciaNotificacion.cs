using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalonDeBelleza.src.models
{
    public class PreferenciaNotificacion
    {
        [Key]
        public int PreferenciaID { get; set; }

        [Required]
        public int UserID { get; set; } // Cliente que establece la preferencia
        [ForeignKey("UserID")]
        public Usuario Usuario { get; set; } // Relación con la tabla Usuarios

        [Required]
        public bool RecibirCorreo { get; set; } = true;

        [Required]
        public bool RecibirWhatsApp { get; set; } = false;
    }

    public class PreferenciaRequest
    {
        public int UserID { get; set; }
        public string Metodo { get; set; }
    }
}
