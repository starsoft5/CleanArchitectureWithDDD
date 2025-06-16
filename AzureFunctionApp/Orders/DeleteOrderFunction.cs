using Application.Commands;
using Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzureFunctionApp.Orders;

public class DeleteOrderFunction
{
    private readonly ILogger<DeleteOrderFunction> _logger;
    private readonly IMediator _mediator;

    public DeleteOrderFunction(ILogger<DeleteOrderFunction> logger,IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [Function(nameof(DeleteOrderFunction))]
    public async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "DeleteOrderFunction/{id}")] HttpRequest req,
    FunctionContext context,
    string id)
    {
        _logger.LogInformation("Processing delete for order with id: {Id}", id);

        // Optional: parse id to int
        if (!int.TryParse(id, out var orderId))
            return new BadRequestObjectResult("Invalid order ID.");

        var result = await _mediator.Send(new DeleteOrderCommand(orderId));

        return new OkObjectResult(result);
    }
}