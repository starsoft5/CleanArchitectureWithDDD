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

namespace AzureFunctionApp;

public class Function1
{
    private readonly ILogger<Function1> _logger;
    private readonly IMediator _mediator;
    private readonly IDistributedCache _cache;



    public Function1(ILogger<Function1> logger, IMediator mediator, IDistributedCache cache)
    {
        _logger = logger;
        _mediator = mediator;
        _cache = cache;
    }

    [Function("Function1")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        var cacheKey = "orders_all";
        var cached = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cached))
        {
            var cachedOrders = JsonSerializer.Deserialize<List<OrderReadDto>>(cached);
            return new OkObjectResult(cachedOrders); // Replace 'Ok' with 'new OkObjectResult'
        }

        var result = await _mediator.Send(new GetAllOrdersQuery());

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        var json = JsonSerializer.Serialize(result);
        await _cache.SetStringAsync(cacheKey, json, options);

        return new OkObjectResult(result); // Replace 'Ok' with 'new OkObjectResult'
    }
}