using Domain.Entities;
using MediatR;
using Application.DTOs;

namespace Application.Commands;

public class CreateOrderCommand : IRequest<OrderReadDto>
{
    public OrderCreateDto Dto { get; }
    public CreateOrderCommand(OrderCreateDto dto) => Dto = dto;

    public CreateOrderCommand()
    {
    }
}