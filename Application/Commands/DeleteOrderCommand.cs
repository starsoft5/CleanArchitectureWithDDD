using MediatR;

namespace Application.Commands;

public class DeleteOrderCommand : IRequest<bool>
{
    public int Id { get; }
    public DeleteOrderCommand(int id) => Id = id;
}
