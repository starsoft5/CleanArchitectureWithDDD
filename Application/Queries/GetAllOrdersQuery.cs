using Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace Application.Queries;

public class GetAllOrdersQuery : IRequest<List<Order>>
{
}
