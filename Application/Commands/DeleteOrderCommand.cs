using MediatR;

namespace Application.Commands;

public record DeleteOrderCommand(int OrderId) : IRequest<bool>;
