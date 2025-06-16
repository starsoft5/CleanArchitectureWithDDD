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

public class GetAllOrdersFunction
{
    private readonly ILogger<GetAllOrdersFunction> _logger;
    private readonly IMediator _mediator;
    private readonly IDistributedCache _cache;



    public GetAllOrdersFunction(ILogger<GetAllOrdersFunction> logger, IMediator mediator, IDistributedCache cache)
    {
        _logger = logger;
        _mediator = mediator;
        _cache = cache;
    }

    [Function("GetAllOrdersFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        _logger.LogInformation("Processing request to retrieve all orders.");

        var cacheKey = "orders_all";
        var cached = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cached))
        {
            _logger.LogInformation("Returning orders from cache.");
            var cachedOrders = JsonSerializer.Deserialize<List<OrderReadDto>>(cached);
            return new OkObjectResult(cachedOrders);
        }

        _logger.LogInformation("Cache miss. Sending GetAllOrdersQuery to MediatR.");

        var result = await _mediator.Send(new GetAllOrdersQuery());

        _logger.LogInformation("Orders retrieved from database. Caching result.");

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        var json = JsonSerializer.Serialize(result);
        await _cache.SetStringAsync(cacheKey, json, options);

        _logger.LogInformation("Returning orders to client.");

        return new OkObjectResult(result);
    }

}