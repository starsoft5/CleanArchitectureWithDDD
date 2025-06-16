using Application.Commands;
using Infrastructure.Data;
using MediatR;

namespace Infrastructure.Handlers;

public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, bool>
{
    private readonly AppDbContext _context;

    public DeleteOrderCommandHandler(AppDbContext context) => _context = context;

    public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.FindAsync(request.Id);
        if (order == null) return false;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
