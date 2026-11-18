namespace Luqma.Domain.Interfaces
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string UserName { get; }

        // Role check against the authenticated user's claims (never against client input)
        bool IsInRole(string role);
    }
}
