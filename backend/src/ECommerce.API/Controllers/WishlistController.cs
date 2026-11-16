using ECommerce.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _svc;
    public WishlistController(IWishlistService svc) => _svc = svc;
    private string UserId => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
        ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct = default) => Ok(await _svc.GetProductIdsAsync(UserId, ct));

    [HttpPost("{productId:guid}")]
    public async Task<IActionResult> Add(Guid productId, CancellationToken ct = default) { await _svc.AddAsync(UserId, productId, ct); return NoContent(); }

    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> Remove(Guid productId, CancellationToken ct = default) { await _svc.RemoveAsync(UserId, productId, ct); return NoContent(); }
}