using Luqma.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.Json;

namespace Luqma.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration config;
        private readonly ILogger<EmailService> logger;
        private readonly IHttpClientFactory httpClientFactory;

        public EmailService(IConfiguration config, ILogger<EmailService> logger, IHttpClientFactory httpClientFactory)
        {
            this.config = config;
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var provider = config["EmailSettings:Provider"] ?? "Smtp";

            // HTTPS API providers work on port 443, which cloud hosts never block,
            // unlike SMTP ports 25/587/465 which are commonly filtered.
            if (provider.Equals("Brevo", StringComparison.OrdinalIgnoreCase))
            {
                await SendViaBrevoAsync(toEmail, subject, body);
                return;
            }

            await SendViaSmtpAsync(toEmail, subject, body);
        }

        private async Task SendViaBrevoAsync(string toEmail, string subject, string body)
        {
            var apiKey = config["EmailSettings:BrevoApiKey"];
            var apiUrl = config["EmailSettings:BrevoApiUrl"] ?? "https://api.brevo.com/v3/smtp/email";
            var senderEmail = config["EmailSettings:SenderEmail"];
            var senderName = config["EmailSettings:SenderName"] ?? "Luqma";

            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(senderEmail))
            {
                throw new InvalidOperationException("EmailSettings:BrevoApiKey/SenderEmail is missing. Cannot send OTP email via Brevo.");
            }

            try
            {
                var client = httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(30);

                var payload = new
                {
                    sender = new { name = senderName, email = senderEmail },
                    to = new[] { new { email = toEmail } },
                    subject,
                    htmlContent = body
                };

                using var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                request.Headers.Add("api-key", apiKey);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                using var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError("Brevo email API failed for {Email}. Status {Status}. Response: {Response}.",
                        toEmail, (int)response.StatusCode, responseBody);
                    throw new InvalidOperationException($"Brevo email API failed with status {(int)response.StatusCode}.");
                }

                logger.LogInformation("OTP email sent via Brevo to {Email} with subject {Subject}.", toEmail, subject);
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                logger.LogError(ex, "Failed to send email via Brevo to {Email} with subject {Subject}.", toEmail, subject);
                throw new InvalidOperationException($"Failed to send email to {toEmail} via Brevo.", ex);
            }
        }

        private async Task SendViaSmtpAsync(string toEmail, string subject, string body)
        {
            var smtpServer = config["EmailSettings:SmtpServer"];
            var portValue = config["EmailSettings:Port"];
            var username = config["EmailSettings:Username"];
            var password = config["EmailSettings:Password"]?.Replace(" ", string.Empty);
            var senderEmail = config["EmailSettings:SenderEmail"];
            var senderName = config["EmailSettings:SenderName"];
            var timeoutMsValue = config["EmailSettings:TimeoutMilliseconds"];

            if (string.IsNullOrWhiteSpace(smtpServer) || string.IsNullOrWhiteSpace(portValue)
                || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)
                || string.IsNullOrWhiteSpace(senderEmail))
            {
                throw new InvalidOperationException("EmailSettings is missing or incomplete. Cannot send OTP email.");
            }

            if (!int.TryParse(portValue, out var port))
            {
                throw new InvalidOperationException($"Invalid EmailSettings:Port value '{portValue}'.");
            }

            if (!int.TryParse(timeoutMsValue, out var timeoutMs) || timeoutMs <= 0)
            {
                timeoutMs = 30000;
            }

            try
            {
                using var smtpClient = new SmtpClient(smtpServer)
                {
                    Port = port,
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Timeout = timeoutMs,
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
                logger.LogInformation("OTP email sent via SMTP {Host}:{Port} to {Email} with subject {Subject}.", smtpServer, port, toEmail, subject);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Failed to send email via SMTP {Host}:{Port} as {User} (timeout {Timeout}ms) to {Email} with subject {Subject}. Error chain: {Chain}.",
                    smtpServer, port, username, timeoutMs, toEmail, subject, Flatten(ex));
                throw new InvalidOperationException($"Failed to send email to {toEmail} via SMTP {smtpServer}:{port}.", ex);
            }
        }

        private static string Flatten(Exception ex)
        {
            var messages = new List<string>();
            for (var current = ex; current != null; current = current.InnerException)
            {
                messages.Add($"{current.GetType().Name}: {current.Message}");
            }
            return string.Join(" <-- ", messages);
        }
    }
}
