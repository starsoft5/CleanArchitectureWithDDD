using Application.Commands;
using Application.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IDistributedCache _cache;

    public OrdersController(IMediator mediator, IDistributedCache cache)
    {
        _mediator = mediator;
        _cache = cache;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var cacheKey = "orders_all";
        var cached = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cached))
        {
            var cachedOrders = JsonSerializer.Deserialize<List<Order>>(cached);
            return Ok(cachedOrders);
        }

        var result = await _mediator.Send(new GetAllOrdersQuery());

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        var json = JsonSerializer.Serialize(result);
        await _cache.SetStringAsync(cacheKey, json, options);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cacheKey = $"order_{id}";
        var cached = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cached))
        {
            var cachedOrder = JsonSerializer.Deserialize<Order>(cached);
            return Ok(cachedOrder);
        }

        var result = await _mediator.Send(new GetOrderByIdQuery(id));
        if (result == null)
            return NotFound();

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        var json = JsonSerializer.Serialize(result);
        await _cache.SetStringAsync(cacheKey, json, options);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Order order)
    {
        var result = await _mediator.Send(new CreateOrderCommand(order));

        // Invalidate cache
        await _cache.RemoveAsync("orders_all");

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Order order)
    {
        if (id != order.Id)
            return BadRequest("Order ID mismatch");

        var result = await _mediator.Send(new UpdateOrderCommand(order));
        if (!result)
            return NotFound();

        // Invalidate caches
        await _cache.RemoveAsync("orders_all");
        await _cache.RemoveAsync($"order_{id}");

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteOrderCommand(id));
        if (!result)
            return NotFound();

        // Invalidate caches
        await _cache.RemoveAsync("orders_all");
        await _cache.RemoveAsync($"order_{id}");

        return NoContent();
    }
}
