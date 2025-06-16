using Application.DTOs;
using Application.Queries;
using Domain.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Handlers;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderReadDto>
{
    private readonly AppDbContext _context;

    public GetOrderByIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OrderReadDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order == null)
            return null;

        return new OrderReadDto
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            OrderDate = order.OrderDate.ToString("yyyy-MM-dd"),
            Items = order.Items.Select(i => new OrderItemReadDto
            {
                Id = i.Id,
                Product = i.Product,
                Quantity = i.Quantity
            }).ToList()
        };
    }
}
