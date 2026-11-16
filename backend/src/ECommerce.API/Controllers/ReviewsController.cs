using ECommerce.Application.DTOs.Reviews;
using ECommerce.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _svc;
    public ReviewsController(IReviewService svc) => _svc = svc;

    [HttpGet("product/{productId:guid}")]
    public async Task<IActionResult> ByProduct(Guid productId, CancellationToken ct = default)
        => Ok(await _svc.GetByProductAsync(productId, ct));

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateReviewRequest request, CancellationToken ct = default)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty;
        var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? User.Identity?.Name ?? "User";
        return Ok(await _svc.CreateAsync(userId, userName, request, ct));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
        => (await _svc.DeleteAsync(id, ct)) ? NoContent() : NotFound();
}