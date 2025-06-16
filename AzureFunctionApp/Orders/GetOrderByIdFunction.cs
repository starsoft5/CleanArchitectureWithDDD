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

public class GetOrderByIdFunction
{
    private readonly ILogger<GetOrderByIdFunction> _logger;
    private readonly IMediator _mediator;
    private readonly IDistributedCache _cache;



    public GetOrderByIdFunction(ILogger<GetOrderByIdFunction> logger, IMediator mediator, IDistributedCache cache)
    {
        _logger = logger;
        _mediator = mediator;
        _cache = cache;
    }

    [Function("GetOrderByIdFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        _logger.LogInformation("Processing request to retrieve order by ID.");

        string idStr = req.Query["id"]!;
        if (string.IsNullOrWhiteSpace(idStr))
        {
            _logger.LogWarning("Missing 'id' parameter in query string.");
            return new BadRequestObjectResult("Please provide an 'id' query parameter.");
        }

        if (!int.TryParse(idStr, out int id))
        {
            _logger.LogWarning("Invalid 'id' parameter: {Id}", idStr);
            return new BadRequestObjectResult("Invalid 'id' query parameter.");
        }

        var cacheKey = "order_by_id";
        var cached = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cached))
        {
            _logger.LogInformation("Returning order with ID {Id} from cache.", id);
            var cachedOrder = JsonSerializer.Deserialize<OrderReadDto>(cached);
            return new OkObjectResult(cachedOrder);
        }

        _logger.LogInformation("Cache miss. Sending GetOrderByIdQuery for ID {Id} to MediatR.", id);

        var result = await _mediator.Send(new GetOrderByIdQuery(id));
        if (result == null)
        {
            _logger.LogWarning("Order with ID {Id} not found.", id);
            return new NotFoundResult();
        }

        _logger.LogInformation("Order retrieved from database. Caching result.");

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        var json = JsonSerializer.Serialize(result);
        await _cache.SetStringAsync(cacheKey, json, options);

        _logger.LogInformation("Returning order with ID {Id} to client.", id);

        return new OkObjectResult(result);
    }


}