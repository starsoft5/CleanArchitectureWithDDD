using Application.Commands;
using Infrastructure.Data;
using Domain.Entities;
using MediatR;
using System;

namespace Infrastructure.Handlers
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Order> // <--- Return Order
    {
        private readonly AppDbContext _context;

        public CreateOrderCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            _context.Orders.Add(request.Order);
            await _context.SaveChangesAsync(cancellationToken);
            return request.Order;
        }
    }
}
