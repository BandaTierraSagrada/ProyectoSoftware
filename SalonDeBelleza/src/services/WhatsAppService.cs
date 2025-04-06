using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SalonDeBelleza.src.config;
using SalonDeBelleza.src.models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
namespace SalonDeBelleza.src.services
{
    public class WhatsAppService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _whatsAppNumber;
        private readonly ApplicationDbContext _context;
        public WhatsAppService(IConfiguration config, ApplicationDbContext context)
        {
            _accountSid = config["Twilio:AccountSID"];
            _authToken = config["Twilio:AuthToken"];
            _whatsAppNumber = config["Twilio:WhatsAppNumber"];
            TwilioClient.Init(_accountSid, _authToken);
            _context = context;
        }

        public async Task EnviarMensajeAsync(string destinatario, string mensaje)
        {
            var message = await MessageResource.CreateAsync(
                body: mensaje,
                from: new Twilio.Types.PhoneNumber(_whatsAppNumber),
                to: new Twilio.Types.PhoneNumber($"whatsapp:{destinatario}")
            );

            Console.WriteLine($"Mensaje enviado: {message.Sid}");
            Console.WriteLine($"Mensaje enviado: {message.Body}");
        }

        public async Task EnviarWhatsAppAdmins(string mensaje)
        {
            var administradores = await _context.Usuarios
                .Where(u => u.Rol == "Administrador")
                .ToListAsync();

            foreach (var admin in administradores)
            {
                // WhatsApp (si se desea)
                if (!string.IsNullOrEmpty(admin.Telefono))
                {
                    await EnviarMensajeAsync("+521" + admin.Telefono, mensaje);

                    _context.Notificaciones.Add(new Notificacion
                    {
                        UserID = admin.UserID,
                        Tipo = "WhatsApp",
                        Destinatario = "+521" + admin.Telefono,
                        Mensaje = mensaje,
                        Enviado = true
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}

