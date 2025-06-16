using Application.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Commands;

public class UpdateOrderCommand : IRequest<bool>
{
    public int Id { get; }
    public OrderUpdateDto Dto { get; }

    public UpdateOrderCommand(int id, OrderUpdateDto dto)
    {
        Id = id;
        Dto = dto;
    }
}