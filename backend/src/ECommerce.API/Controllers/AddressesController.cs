using ECommerce.Application.DTOs.Addresses;
using ECommerce.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AddressesController : ControllerBase
{
    private readonly IAddressService _svc;
    public AddressesController(IAddressService svc) => _svc = svc;
    private string UserId => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
        ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct = default) => Ok(await _svc.GetByUserAsync(UserId, ct));

    [HttpPost]
    public async Task<IActionResult> Create(CreateAddressRequest request, CancellationToken ct = default) => Ok(await _svc.CreateAsync(UserId, request, ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, CreateAddressRequest request, CancellationToken ct = default)
    {
        var r = await _svc.UpdateAsync(UserId, id, request, ct);
        return r == null ? NotFound() : Ok(r);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
        => (await _svc.DeleteAsync(UserId, id, ct)) ? NoContent() : NotFound();
}