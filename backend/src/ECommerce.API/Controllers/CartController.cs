using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _svc;
    public CartController(ICartService svc) => _svc = svc;

    private string UserId => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
        ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct = default) => Ok(await _svc.GetCartAsync(UserId, ct));

    [HttpPost("items")]
    public async Task<IActionResult> Add(AddToCartRequest request, CancellationToken ct = default) => Ok(await _svc.AddItemAsync(UserId, request, ct));

    [HttpPut("items/{itemId:guid}")]
    public async Task<IActionResult> Update(Guid itemId, UpdateCartItemRequest request, CancellationToken ct = default) => Ok(await _svc.UpdateItemAsync(UserId, itemId, request, ct));

    [HttpDelete("items/{itemId:guid}")]
    public async Task<IActionResult> Remove(Guid itemId, CancellationToken ct = default) => Ok(await _svc.RemoveItemAsync(UserId, itemId, ct));

    [HttpDelete]
    public async Task<IActionResult> Clear(CancellationToken ct = default) { await _svc.ClearAsync(UserId, ct); return NoContent(); }
}