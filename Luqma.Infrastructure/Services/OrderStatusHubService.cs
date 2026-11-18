//using Luqma.Domain.Interfaces;
//using Luqma.Infrastructure.Hubs;
//// using Microsoft.AspNetCore.SignalR;

//namespace Luqma.Infrastructure.Services
//{
//    public class OrderStatusHubService : IOrderStatusHub
//    {
//        private readonly IHubContext<OrderStatusHub> _hubContext;

//        public OrderStatusHubService(IHubContext<OrderStatusHub> hubContext)
//        {
//            _hubContext = hubContext;
//        }

//        public async Task SendOrderStatusUpdate(string userId, object data)
//        {
//            await _hubContext.Clients
//                .Group($"user_{userId}")
//                .SendAsync("OrderStatusUpdated", data);
//        }
//    }
//}
