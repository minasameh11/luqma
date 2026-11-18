using Luqma.Domain.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Luqma.Infrastructure.Services
{
    public class FirebaseService : IFirebaseService
    {
        private readonly ILogger<FirebaseService> _logger;

        public FirebaseService(IConfiguration configuration, ILogger<FirebaseService> logger)
        {
            _logger = logger;

            // نهيئ Firebase App مرة واحدة بس طول عمر التطبيق
            if (FirebaseApp.DefaultInstance == null)
            {
                var credentialPath = configuration["Firebase:CredentialPath"]
                    ?? Path.Combine(AppContext.BaseDirectory, "firebase-adminsdk.json");

                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(credentialPath)
                });
            }
        }

        public async Task SendToTokenAsync(
            string deviceToken,
            string title,
            string body,
            string? imageUrl = null,
            Dictionary<string, string>? data = null)
        {
            try
            {
                var message = new Message
                {
                    Token = deviceToken,
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body,
                        ImageUrl = imageUrl
                    },
                    Data = data ?? new Dictionary<string, string>(),
                    Android = new AndroidConfig
                    {
                        Priority = Priority.High,
                        Notification = new AndroidNotification
                        {
                            Sound = "default",
                            ClickAction = "FLUTTER_NOTIFICATION_CLICK"
                        }
                    },
                    Apns = new ApnsConfig
                    {
                        Aps = new Aps { Sound = "default" }
                    }
                };

                var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                _logger.LogInformation("FCM notification sent. MessageId: {MessageId}", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send FCM notification to token: {Token}", deviceToken);
            }
        }

        public async Task SendToMultipleAsync(
            List<string> deviceTokens,
            string title,
            string body,
            string? imageUrl = null,
            Dictionary<string, string>? data = null)
        {
            if (deviceTokens == null || deviceTokens.Count == 0) return;

            try
            {
                // FCM v1 بتبعت max 500 token في طلب واحد
                var batches = deviceTokens
                    .Select((token, index) => new { token, index })
                    .GroupBy(x => x.index / 500)
                    .Select(g => g.Select(x => x.token).ToList())
                    .ToList();

                foreach (var batch in batches)
                {
                    var multicastMessage = new MulticastMessage
                    {
                        Tokens = batch,
                        Notification = new Notification
                        {
                            Title = title,
                            Body = body,
                            ImageUrl = imageUrl
                        },
                        Data = data ?? new Dictionary<string, string>(),
                        Android = new AndroidConfig
                        {
                            Priority = Priority.High,
                            Notification = new AndroidNotification
                            {
                                Sound = "default",
                                ClickAction = "FLUTTER_NOTIFICATION_CLICK"
                            }
                        },
                        Apns = new ApnsConfig
                        {
                            Aps = new Aps { Sound = "default" }
                        }
                    };

                    var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(multicastMessage);
                    _logger.LogInformation(
                        "FCM multicast sent. Success: {SuccessCount}, Failure: {FailureCount}",
                        response.SuccessCount,
                        response.FailureCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send FCM multicast notification");
            }
        }
    }
}

