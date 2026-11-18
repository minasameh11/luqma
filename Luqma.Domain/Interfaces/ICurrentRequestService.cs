namespace Luqma.Domain.Interfaces
{
    public interface ICurrentRequestService
    {
        // Public base URL of the running API (without a trailing slash), e.g. https://luqma-api.runasp.net
        string BaseUrl { get; }
    }
}
