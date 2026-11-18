namespace Luqma.Domain.Interfaces
{
    public interface IFirebaseService
    {
        Task SendToTokenAsync(string deviceToken, string title, string body, string? imageUrl = null, Dictionary<string, string>? data = null);
        Task SendToMultipleAsync(List<string> deviceTokens, string title, string body, string? imageUrl = null, Dictionary<string, string>? data = null);
    }
}
