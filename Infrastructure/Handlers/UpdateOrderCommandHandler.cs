using Application.Commands;
using Domain.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Handlers;

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, bool>
{
    private readonly AppDbContext _context;

    public UpdateOrderCommandHandler(AppDbContext context) => _context = context;

    public async Task<bool> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Load the order and its items
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order == null) return false;

        // Update order properties
        order.CustomerName = dto.CustomerName;
        order.OrderDate = dto.OrderDate;

        // Clear existing items
        _context.OrderItems.RemoveRange(order.Items);

        // Add new items from DTO
        order.Items = dto.Items.Select(i => new OrderItem
        {
            Product = i.Product,
            Quantity = i.Quantity,
            Price = i.Price
        }).ToList();

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

}