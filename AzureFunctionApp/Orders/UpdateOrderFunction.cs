using Application.Commands;
using Application.DTOs;
using Application.Queries;
using Infrastructure.Handlers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AzureFunctionApp.Orders;

public class UpdateOrderFunction
{
    private readonly ILogger<UpdateOrderFunction> _logger;
    private readonly IMediator _mediator;
    private readonly IDistributedCache _cache;



    public UpdateOrderFunction(ILogger<UpdateOrderFunction> logger, IMediator mediator, IDistributedCache cache)
    {
        _logger = logger;
        _mediator = mediator;
        _cache = cache;
    }

    [Function("UpdateOrderFunction")]
    public async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "UpdateOrderFunction/{id}")] HttpRequest req,
    FunctionContext context,
    string id)
    {
        _logger.LogInformation("Processing update for order with id: {Id}", id);

        // Optional: parse id to int
        if (!int.TryParse(id, out var orderId))
            return new BadRequestObjectResult("Invalid order ID.");

        // Read body and deserialize your DTO, then process update logic
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var dto = JsonSerializer.Deserialize<OrderUpdateDto>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (dto == null)
            return new BadRequestObjectResult("Invalid request body.");

        var result = await _mediator.Send(new UpdateOrderCommand(orderId, dto));

        return new OkObjectResult(result);
    }
    }
 