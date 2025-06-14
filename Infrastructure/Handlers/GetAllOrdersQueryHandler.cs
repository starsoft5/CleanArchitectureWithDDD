using Application.Queries;
using Domain.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Handlers;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, List<Order>>
{
    private readonly AppDbContext _context;

    public GetAllOrdersQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Order>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .ToListAsync(cancellationToken);
    }
}
