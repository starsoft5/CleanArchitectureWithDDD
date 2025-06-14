using Domain.Entities;
using MediatR;

namespace Application.Queries
{
    public record GetOrderByIdQuery(int Id) : IRequest<Order>;
}
