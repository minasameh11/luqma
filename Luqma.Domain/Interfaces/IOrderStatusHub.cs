namespace Luqma.Domain.Interfaces
{
    public interface IOrderStatusHub
    {
        Task SendOrderStatusUpdate(string userId, object data);
    }
}
