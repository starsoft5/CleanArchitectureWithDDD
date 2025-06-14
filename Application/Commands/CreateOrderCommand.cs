using Domain.Entities;
using MediatR;

namespace Application.Commands;

public record CreateOrderCommand(Order Order) : IRequest<Order>; // <--- Must return Order
