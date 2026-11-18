using Luqma.Application.Wapper;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Luqma.Application.Features.Commands.CartCommands.ClearCartCommand
{
    public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, Response<int>>
    {
        private readonly IGenericRepository<CartItem> _cartItems;
        private readonly ICurrentUserService _currentUser;

        public ClearCartCommandHandler(IGenericRepository<CartItem> cartItems, ICurrentUserService currentUser)
        {
            _cartItems = cartItems;
            _currentUser = currentUser;
        }

        public async Task<Response<int>> Handle(ClearCartCommand command, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUser.UserId, out var userId))
                return Response<int>.Fail("غير مصرح");

            var lines = await _cartItems.GetAll()
                .Where(c => c.UserId == userId && !c.IsArchived)
                .ToListAsync(cancellationToken);

            if (lines.Count == 0)
                return Response<int>.Success(0, "السلة فارغة بالفعل");

            foreach (var line in lines)
                _cartItems.Archived(line);

            if (!await _cartItems.SaveChangesAsync())
                return Response<int>.Fail("فشل في تفريغ السلة");

            return Response<int>.Success(lines.Count, "تم تفريغ السلة");
        }
    }
}
