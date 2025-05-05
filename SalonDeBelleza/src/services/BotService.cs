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

        public BotService(ApplicationDbContext context)
        {
            _context = context;
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
            Console.WriteLine("Entro a procesar");
            Console.WriteLine($"{numero} {body}");
            int clienteid = await GetUsuarioPorTelefono(numero);
            if (clienteid == 0) return "No estas registrado, ve al sitio web a registrarte";
            var estado = await _context.EstadosConversacion
            .FirstOrDefaultAsync(e => e.TelefonoUsuario == numero);
            body = body.Trim().ToLower();
            List<string> servicios = ["Corte", "Tintes", "Manicura", "Pedicura"];
            if (estado == null)
            {
                estado = new EstadoConversacion
                {
                    TelefonoUsuario = numero,
                    PasoActual = PasoConversacion.Inicio
                };
                _context.EstadosConversacion.Add(estado);
                await _context.SaveChangesAsync();
            }
            Console.WriteLine("Aqui si pasa");
            if(body == "cancelar")
            {
                _context.EstadosConversacion.Remove(estado);
                await _context.SaveChangesAsync();
                return "Saliendo...";
            }

            if (body == "agendar")
            {
                estado.PasoActual = PasoConversacion.Fecha;
                await _context.SaveChangesAsync();
                return "📅 Por favor, escribe la fecha (formato: YYYY-MM-DD):";
            }

            if (estado.PasoActual == PasoConversacion.Fecha)
            {
                Console.WriteLine("Entro a seleccionar servicio");
                if (DateTime.TryParse(body, out DateTime fecha))
                {
                    estado.Fecha = fecha;
                    estado.PasoActual = PasoConversacion.Servicio;
                    await _context.SaveChangesAsync();
                    string lista = "💇‍♀️ Escribe el nombre del servicio:\n";
                    for (int i = 0; i < servicios.Count; i++)
                        lista += $"{i + 1}. {servicios[i]}\n";
                    return lista;
                }
                return "❌ Fecha inválida. Intenta con el formato YYYY-MM-DD.";
            }

            if (estado.PasoActual == PasoConversacion.Servicio)
            {
                estado.Servicio = body;
                if (!servicios.Contains(body))
                {
                    return "Servicio invalido";
                }
                estado.PasoActual = PasoConversacion.Colaborador;
                await _context.SaveChangesAsync();

                var colaboradores = await _context.Colaboradores
                    .Include(c => c.Usuario)
                    .Where(c => c.TipoServicio.ToLower() == body.ToLower())
                    .ToListAsync();

                if (!colaboradores.Any())
                    return "❌ No se encontraron colaboradores para ese servicio.";

                estado.PasoActual = PasoConversacion.Colaborador;

                string lista = "👩‍🔧 Elige una colaboradora (escribe el número):\n";
                for (int i = 0; i < colaboradores.Count; i++)
                    lista += $"{i + 1}. {colaboradores[i].Usuario.Nombre}\n";

                return lista;
            }

            if (estado.PasoActual == PasoConversacion.Colaborador)
            {
                var colaboradores = await _context.Colaboradores
                    .Include(c => c.Usuario)
                    .Where(c => c.TipoServicio.ToLower() == estado.Servicio.ToLower())
                    .ToListAsync();

                if (int.TryParse(body, out int index) && index >= 1 && index <= colaboradores.Count)
                {
                    var colaborador = colaboradores[index - 1];
                    estado.ColaboradorID = colaborador.UserID;
                    estado.PasoActual = PasoConversacion.Hora;
                    await _context.SaveChangesAsync();

                    // Buscar horarios disponibles
                    var citas = await _context.Citas
                        .Where(c => c.ColaboradorID == colaborador.UserID && c.FechaHora.Date == estado.Fecha.Date)
                        .Select(c => c.FechaHora.TimeOfDay)
                        .ToListAsync();

                    var disponibles = new List<string>();
                    var hora = colaborador.HorarioEntrada;
                    var duracion = TimeSpan.FromMinutes(colaborador.DuracionServicio);

                    while (hora + duracion <= colaborador.HorarioSalida)
                    {
                        if (!citas.Any(c => Math.Abs((c - hora).TotalMinutes) < 1))
                            disponibles.Add(hora.ToString(@"hh\:mm"));
                        hora += duracion;
                    }

                    if (!disponibles.Any())
                        return "❌ No hay horarios disponibles ese día.";


                    estado.ColaboradorID = colaborador.UserID; // ya se guardó
                    await _context.SaveChangesAsync();

                    string horarios = "⏰ Horas disponibles:\n";
                    for (int i = 0; i < disponibles.Count; i++)
                        horarios += $"{i + 1}. {disponibles[i]}\n";

                    return horarios + "\nEscribe el número de la hora que deseas:";
                }
                return "❌ Selección inválida. Escribe el número correspondiente.";
            }

            if (estado.PasoActual == PasoConversacion.Hora)
            {
                var colaborador = await _context.Colaboradores
                    .Include(c => c.Usuario)
                    .FirstOrDefaultAsync(c => c.Usuario.UserID == estado.ColaboradorID);

                if (colaborador == null)
                    return "❌ No se pudo encontrar la colaboradora seleccionada.";

                var citas = await _context.Citas
                    .Where(c => c.ColaboradorID == colaborador.UserID && c.FechaHora.Date == estado.Fecha.Date)
                    .Select(c => c.FechaHora.TimeOfDay)
                    .ToListAsync();

                var disponibles = new List<string>();
                var hora = colaborador.HorarioEntrada;
                var duracion = TimeSpan.FromMinutes(colaborador.DuracionServicio);

                while (hora + duracion <= colaborador.HorarioSalida)
                {
                    if (!citas.Any(c => Math.Abs((c - hora).TotalMinutes) < 1))
                        disponibles.Add(hora.ToString(@"hh\:mm"));
                    hora += duracion;
                }

                if (int.TryParse(body, out int index) && index >= 1 && index <= disponibles.Count)
                {
                    var horaSeleccionada = TimeSpan.Parse(disponibles[index - 1]);
                    var fechaHora = estado.Fecha.Date + horaSeleccionada;

                    var cliente = await _context.Usuarios
                        .FirstOrDefaultAsync(c => c.Telefono == numero);

                    if (cliente == null)
                        return "❌ No se encontró tu cuenta como cliente.";

                    Cita cita = new Cita 
                    { 
                        ClienteID = clienteid,
                        ColaboradorID = colaborador.UserID,
                        FechaHora = fechaHora,
                        Servicio = estado.Servicio,
                        Estado = "Pendiente",
                        Notas = "Desde WhatsApp"
                    };

                    _context.Citas.Add(cita);
                    _context.EstadosConversacion.Remove(estado);
                    await _context.SaveChangesAsync();

                    return $"✅ Cita agendada el {fechaHora:dd/MM/yyyy HH:mm} con {colaborador.Usuario.Nombre}.";
                }
                return "❌ Selección inválida.";
            }

            if (body == "ver citas")
            {
                var citas = await _context.Citas
                    .Where(c => c.ClienteID == clienteid && c.Estado == "Pendiente")
                    .ToListAsync();

                if (!citas.Any())
                    return "No tienes citas pendientes.";

                string lista = "📋 Tus citas:\n";
                foreach (var cita in citas)
                {
                    lista += $"Cita #{cita.CitaID} - {cita.FechaHora:dd/MM/yyyy HH:mm}\nServicio: {cita.Servicio}\nEscribe:\n*confirmar {cita.CitaID}* o *cancelar {cita.CitaID}*\n\n\n";
                }
                return lista;
            }
           
            return "Hola 👋\nOpciones disponibles:\n- *agendar* 📆\n- *ver citas* 📋";
        }
        
    }
}
