namespace SalonDeBelleza.src.models
{
    public class EstadoConversacion
    {
        public PasoConversacion Paso { get; set; } = PasoConversacion.Inicio;
        public DateTime Fecha { get; set; }
        public string Servicio { get; set; }
        public List<ColaboradorInfo> Colaboradores { get; set; }
        public ColaboradorInfo Colaborador { get; set; }
        public List<string> HorasDisponibles { get; set; }

        public void Reset()
        {
            Paso = PasoConversacion.Inicio;
            Fecha = DateTime.Now;
            Servicio = null;
            Colaboradores = new List<ColaboradorInfo>();
            Colaborador = null;
            HorasDisponibles = new List<string>();
        }
        public override string ToString()
        {
            return $"PasoActual: {Paso}, Fecha: {Fecha}, Servicio: {Servicio}, Colaborador: {Colaborador}";
        }
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
