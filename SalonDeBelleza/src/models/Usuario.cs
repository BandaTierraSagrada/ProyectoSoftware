using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalonDeBelleza.src.models
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(15)]
        public string Telefono { get; set; }

        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(255)]
        public string Password { get; set; } // Se almacena como hash

        public string LoginStatus { get; set; } = "Inactivo";

        [StringLength(50)]
        public string Rol { get; set; } // Cliente, Administrador, Colaborador
    }
}
