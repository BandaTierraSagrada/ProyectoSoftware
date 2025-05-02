namespace SalonDeBelleza.src.models
{
    public class AgendarCitaRequest
    {
        public string TelefonoCliente { get; set; }
        public string Servicio { get; set; }
        public int ColaboradorID { get; set; }
        public DateTime FechaHora { get; set; }
        public string Notas { get; set; }
    }
}
