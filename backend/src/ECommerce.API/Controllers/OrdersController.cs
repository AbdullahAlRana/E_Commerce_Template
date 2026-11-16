using ECommerce.Application.DTOs.Orders;
using ECommerce.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _svc;
    public OrdersController(IOrderService svc) => _svc = svc;

    private string UserId => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
        ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty;
    private bool IsAdmin => User.IsInRole("Admin");

    [HttpGet("my")]
    public async Task<IActionResult> MyOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        => Ok(await _svc.GetMyOrdersAsync(UserId, page, pageSize, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct = default)
    {
        var r = await _svc.GetByIdAsync(id, UserId, IsAdmin, ct);
        return r == null ? NotFound() : Ok(r);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest request, CancellationToken ct = default)
        => Ok(await _svc.CreateOrderAsync(UserId, request, ct));

    // Admin
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        => Ok(await _svc.GetAllAsync(page, pageSize, ct));

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateOrderStatusRequest request, CancellationToken ct = default)
    {
        var r = await _svc.UpdateStatusAsync(id, request, ct);
        return r == null ? NotFound() : Ok(r);
    }
}