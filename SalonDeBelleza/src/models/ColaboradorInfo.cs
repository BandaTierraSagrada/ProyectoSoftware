using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SalonDeBelleza.src.models
{
    public class ColaboradorInfo
    {
        [Key, ForeignKey("Usuario")]
        public int UserID { get; set; } // Se usa UserID como clave primaria
        public virtual Usuario Usuario { get; set; }

        public TimeSpan HorarioEntrada { get; set; }

        public TimeSpan HorarioSalida { get; set; }

        public string TipoServicio { get; set; }

        public int DuracionServicio { get; set; } // En minutos
    }
}
