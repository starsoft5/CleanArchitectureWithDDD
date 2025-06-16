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

public class CreateOrderFunction
{
    private readonly ILogger<CreateOrderFunction> _logger;
    private readonly IMediator _mediator;
    private readonly IDistributedCache _cache;



    public CreateOrderFunction(ILogger<CreateOrderFunction> logger, IMediator mediator, IDistributedCache cache)
    {
        _logger = logger;
        _mediator = mediator;
        _cache = cache;
    }

    [Function("CreateOrderFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        _logger.LogInformation("Processing request to create order.");
        var cacheKey = "orders_all";
        var cacheKey2 = "order_by_id";

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var dto = JsonSerializer.Deserialize<OrderCreateDto>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (dto == null)
        {
            _logger.LogWarning("Invalid request body.");
            return new BadRequestObjectResult("Invalid request body.");
        }

        var result = await _mediator.Send(new CreateOrderCommand(dto));
        if (result == null)
        {
            _logger.LogWarning("Order create failed.");
            return new NotFoundResult();
        }

        await _cache.RemoveAsync(cacheKey);
        await _cache.RemoveAsync(cacheKey2);

        _logger.LogInformation("Creating order successful.");

        return new OkObjectResult(result);
    }


}