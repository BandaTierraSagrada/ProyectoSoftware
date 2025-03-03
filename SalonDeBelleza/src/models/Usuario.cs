using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SalonDeBelleza.src.models
{
    public class Usuario : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(15)]
        public string Telefono { get; set; }

        [StringLength(255)]
        public string Email { get; set; } // Este campo es opcional

        [Required]
        public string Loginstatus { get; set; } = "Inactivo"; // Estado de inicio de sesión

        [Required]
        public string Rol { get; set; } = "Cliente"; // Rol por defecto
    }
}