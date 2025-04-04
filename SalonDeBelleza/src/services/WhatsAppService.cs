using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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
        public WhatsAppService(IConfiguration config)
        {
            _accountSid = config["Twilio:AccountSID"];
            _authToken = config["Twilio:AuthToken"];
            _whatsAppNumber = config["Twilio:WhatsAppNumber"];
            TwilioClient.Init(_accountSid, _authToken);
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
    }
}

