using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using SalonDeBelleza.src.models;
namespace SalonDeBelleza.src.services
{
    public class EmailService
    {
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string refreshToken;
        private readonly string emailFrom;

        public EmailService(IConfiguration config)
        {
            clientId = config["GmailOAuth:ClientId"];
            clientSecret = config["GmailOAuth:ClientSecret"];
            refreshToken = config["GmailOAuth:RefreshToken"];
            emailFrom = config["GmailOAuth:EmailFrom"];
        }

        public async Task<bool> EnviarCorreoAsync(NotificacionRequest request)
        {
            try
            {
                var credential = new UserCredential(new GoogleAuthorizationCodeFlow(
                    new GoogleAuthorizationCodeFlow.Initializer
                    {
                        ClientSecrets = new ClientSecrets
                        {
                            ClientId = clientId,
                            ClientSecret = clientSecret
                        }
                    }), "user", new TokenResponse { RefreshToken = refreshToken });

                await credential.RefreshTokenAsync(CancellationToken.None);

                var service = new GmailService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "SalonDeBelleza"
                });
                // 🟡 Aquí usamos la plantilla
                string htmlCuerpo = await ObtenerContenidoCorreoAsync(request.Nombre, request.CuerpoHtml);
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Salon de Belleza", emailFrom));
                message.To.Add(new MailboxAddress(request.Destinatario, request.Destinatario));
                message.Subject = request.Asunto;

                //var bodyBuilder = new BodyBuilder { HtmlBody = request.CuerpoHtml };
                var bodyBuilder = new BodyBuilder { HtmlBody = htmlCuerpo };
                message.Body = bodyBuilder.ToMessageBody();

                using (var stream = new MemoryStream())
                {
                    await message.WriteToAsync(stream);
                    var rawMessage = Convert.ToBase64String(stream.ToArray())
                        .Replace('+', '-')
                        .Replace('/', '_')
                        .Replace("=", "");

                    var gmailMessage = new Message { Raw = rawMessage };
                    await service.Users.Messages.Send(gmailMessage, "me").ExecuteAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar correo: {ex.Message}");
                return false;
            }
        }

        public async Task<string> ObtenerContenidoCorreoAsync(string nombre, string mensaje)
        {
            string rutaPlantilla = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", "plantillascorreo", "citaconfirmada.html");
            string html = await File.ReadAllTextAsync(rutaPlantilla);

            html = html.Replace("{{NOMBRE}}", nombre);
            html = html.Replace("{{MENSAJE}}", mensaje);

            return html;
        }


    }
}
