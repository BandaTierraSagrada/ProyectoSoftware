using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalonDeBelleza.src.models
{
    public class Cita
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CitaID { get; set; }

        [ForeignKey("Usuario")]
        public int ClienteID { get; set; }
        public Usuario Cliente { get; set; }  // Relación con Usuario (Cliente)

        [ForeignKey("Usuario")]
        public int ColaboradorID { get; set; }
        public Usuario Colaborador { get; set; } // Relación con Usuario (Colaborador)

        [Required]
        public DateTime FechaHora { get; set; } // Fecha y hora de la cita

        [Required]
        [StringLength(100)]
        public string Servicio { get; set; }  // Tipo de servicio seleccionado

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente";  // Estado de la cita (Pendiente, Confirmada, Cancelada, Completada)

        [StringLength(500)]
        public string Notas { get; set; } // Notas adicionales
    }
}
