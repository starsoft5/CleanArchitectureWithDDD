using Application.Commands;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Handlers;

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, bool>
{
    private readonly AppDbContext _context;

    public UpdateOrderCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var existingOrder = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.Order.Id, cancellationToken);

        if (existingOrder == null)
            return false;

        // Update properties of the order
        existingOrder.CustomerName = request.Order.CustomerName;
        existingOrder.OrderDate = request.Order.OrderDate;

        // Update order items (simple replacement for demo)
        _context.OrderItems.RemoveRange(existingOrder.Items);
        existingOrder.Items = request.Order.Items;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
