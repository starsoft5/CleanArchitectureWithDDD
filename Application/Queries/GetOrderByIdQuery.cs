using Application.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Queries
{
    public class GetOrderByIdQuery : IRequest<OrderReadDto>
    {
        public int Id { get; }
        public GetOrderByIdQuery(int id) => Id = id;
    }
}
