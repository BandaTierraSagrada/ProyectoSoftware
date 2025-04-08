using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SalonDeBelleza.src.models
{
    public class Notificacion
    {
        [Key]
        public int NotificacionID { get; set; }

        [Required]
        public int UserID { get; set; }  // Cliente que recibe la notificación
        [ForeignKey("UserID")]
        public Usuario Usuario { get; set; } // Relación con la tabla Usuarios

        [Required]
        public string Tipo { get; set; } // "Correo" o "WhatsApp"

        [Required]
        public string Destinatario { get; set; } // Email o número de teléfono

        [Required]
        public string Mensaje { get; set; }

        public DateTime FechaEnvio { get; set; } = DateTime.Now;

        public bool Enviado { get; set; } = false; // Si se envió correctamente
    }
    public class NotificacionRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Destinatario { get; set; } = string.Empty;
        public string Asunto { get; set; } = string.Empty;
        public string CuerpoHtml { get; set; } = string.Empty;
    }
    public class WhatsAppRequest
    {
        public string Destinatario { get; set; }
        public string Mensaje { get; set; }
    }
}
