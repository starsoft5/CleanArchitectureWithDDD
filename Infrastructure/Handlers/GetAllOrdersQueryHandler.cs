using Application.DTOs;
using Application.Queries;
using Domain.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Handlers;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, List<OrderReadDto>>
{
    private readonly AppDbContext _context;
    public GetAllOrdersQueryHandler(AppDbContext context) => _context = context;

    public async Task<List<OrderReadDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Orders
                .Include(o => o.Items) // assumes navigation property exists
                .Select(o => new OrderReadDto
                {
        Id = o.Id,
        CustomerName = o.CustomerName,
        OrderDate = o.OrderDate.ToString("yyyy-MM-dd"),
        Items = o.Items.Select(i => new OrderItemReadDto
        {
            Id = i.Id,
            Product = i.Product,
            Quantity = i.Quantity,
            Price = i.Price
        }).ToList()
    }).ToListAsync(cancellationToken);
    }
}