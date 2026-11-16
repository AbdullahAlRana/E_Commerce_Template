using ECommerce.Application.DTOs.Coupons;
using ECommerce.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CouponsController : ControllerBase
{
    private readonly ICouponService _svc;
    public CouponsController(ICouponService svc) => _svc = svc;

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        => Ok(await _svc.GetAllAsync(page, pageSize, ct));

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateCouponRequest request, CancellationToken ct = default) => Ok(await _svc.CreateAsync(request, ct));

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCouponRequest request, CancellationToken ct = default)
    {
        var r = await _svc.UpdateAsync(id, request, ct);
        return r == null ? NotFound() : Ok(r);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
        => (await _svc.DeleteAsync(id, ct)) ? NoContent() : NotFound();

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(ValidateCouponRequest request, CancellationToken ct = default)
        => Ok(await _svc.ValidateAsync(request, ct));
}