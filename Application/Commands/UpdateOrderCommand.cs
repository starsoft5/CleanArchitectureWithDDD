using Domain.Entities;
using MediatR;

namespace Application.Commands;

public record UpdateOrderCommand(Order Order) : IRequest<bool>;
