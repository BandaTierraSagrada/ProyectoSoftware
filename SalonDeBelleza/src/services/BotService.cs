using Microsoft.EntityFrameworkCore;
using SalonDeBelleza.src.config;
using SalonDeBelleza.src.models;
using System;
using System.Threading.Tasks;
using Twilio.Http;

namespace SalonDeBelleza.src.services
{
    public class BotService
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<string, EstadoConversacion> _conversaciones;

        public BotService(ApplicationDbContext context)
        {
            _context = context;
            _conversaciones = new Dictionary<string, EstadoConversacion>();
        }

        public async Task<List<Cita>> ObtenerCitasPendientesPorTelefono(string telefono)
        {
            return await _context.Citas
                .Include(c => c.Cliente)
                .Where(c => c.Cliente.Telefono == telefono && c.Estado == "Pendiente")
                .ToListAsync();
        }

        public async Task<bool> ConfirmarOCancelarCitaAsync(int citaId, string estado)
        {
            var cita = await _context.Citas.FindAsync(citaId);
            if (cita == null) return false;

            cita.Estado = estado;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetUsuarioPorTelefono(string telefono)
        {
            Usuario usuario = await _context.Usuarios.FirstOrDefaultAsync(n => n.Telefono == telefono);
            if (usuario == null) return 0;
            return usuario.UserID;
        }


        public async Task<string> ProcesarMensajeAsync(string numero, string body)
        {
            if (!_conversaciones.ContainsKey(numero))
                _conversaciones[numero] = new EstadoConversacion();
            var estado = _conversaciones[numero];

            body = body.Trim().ToLower();

            if (body == "agendar")
            {
                estado.Reset();
                estado.Paso = PasoConversacion.Fecha;
                return "📅 Por favor, escribe la fecha (formato: YYYY-MM-DD):";
            }

            if (estado.Paso == PasoConversacion.Fecha)
            {
                if (DateTime.TryParse(body, out DateTime fecha))
                {
                    estado.Fecha = fecha;
                    estado.Paso = PasoConversacion.Servicio;
                    return "💇‍♀️ Escribe el nombre del servicio:";
                }
                return "❌ Fecha inválida. Intenta con el formato YYYY-MM-DD.";
            }

            if (estado.Paso == PasoConversacion.Servicio)
            {
                estado.Servicio = body;
                var colaboradores = await _context.Colaboradores
                    .Include(c => c.Usuario)
                    .Where(c => c.TipoServicio.ToLower() == body)
                    .ToListAsync();

                if (!colaboradores.Any())
                    return "❌ No se encontraron colaboradores para ese servicio.";

                estado.Colaboradores = colaboradores;
                estado.Paso = PasoConversacion.Colaborador;

                string lista = "👩‍🔧 Elige una colaboradora (escribe el número):\n";
                for (int i = 0; i < colaboradores.Count; i++)
                    lista += $"{i + 1}. {colaboradores[i].Usuario.Nombre}\n";

                return lista;
            }

            if (estado.Paso == PasoConversacion.Colaborador)
            {
                if (int.TryParse(body, out int index) && index >= 1 && index <= estado.Colaboradores.Count)
                {
                    estado.Colaborador = estado.Colaboradores[index - 1];

                    // Buscar horarios disponibles
                    var citas = await _context.Citas
                        .Where(c => c.ColaboradorID == estado.Colaborador.UserID && c.FechaHora.Date == estado.Fecha.Date)
                        .Select(c => c.FechaHora.TimeOfDay)
                        .ToListAsync();

                    var disponibles = new List<string>();
                    var hora = estado.Colaborador.HorarioEntrada;
                    var duracion = TimeSpan.FromMinutes(estado.Colaborador.DuracionServicio);

                    while (hora + duracion <= estado.Colaborador.HorarioSalida)
                    {
                        if (!citas.Any(c => Math.Abs((c - hora).TotalMinutes) < 1))
                            disponibles.Add(hora.ToString(@"hh\:mm"));
                        hora += duracion;
                    }

                    if (!disponibles.Any())
                        return "❌ No hay horarios disponibles ese día.";

                    estado.HorasDisponibles = disponibles;
                    estado.Paso = PasoConversacion.Hora;

                    string horarios = "⏰ Horas disponibles:\n";
                    for (int i = 0; i < disponibles.Count; i++)
                        horarios += $"{i + 1}. {disponibles[i]}\n";

                    return horarios + "\nEscribe el número de la hora que deseas:";
                }
                return "❌ Selección inválida. Escribe el número correspondiente.";
            }

            if (estado.Paso == PasoConversacion.Hora)
            {
                if (int.TryParse(body, out int index) && index >= 1 && index <= estado.HorasDisponibles.Count)
                {
                    var horaSeleccionada = TimeSpan.Parse(estado.HorasDisponibles[index - 1]);
                    var fechaHora = estado.Fecha.Date + horaSeleccionada;
                    int clienteid = await GetUsuarioPorTelefono(numero);
                    if (clienteid == 0) return "No estas registrado, ve al sitio web a registrarte";
                    Cita cita = new Cita 
                    { 
                        ClienteID = clienteid,
                        ColaboradorID = estado.Colaborador.UserID,
                        FechaHora = fechaHora,
                        Servicio = estado.Servicio,
                        Estado = "Pendiente"

                    };

                    _context.Citas.Add(cita);
                    await _context.SaveChangesAsync();

                    _conversaciones.Remove(numero);

                    return $"✅ Cita agendada el {fechaHora:dd/MM/yyyy HH:mm} con {estado.Colaborador.Usuario.Nombre}.";
                }
                return "❌ Selección inválida.";
            }



            return "Hola 👋\nOpciones disponibles:\n- *agendar* 📆\n- *ver citas* 📋";
        }
    }
}
