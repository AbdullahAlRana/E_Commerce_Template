using ECommerce.Application.Common;
using ECommerce.Application.DTOs.Users;
using ECommerce.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _svc;
    public UsersController(IUserService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        => Ok(await _svc.GetAllAsync(page, pageSize, ct));

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id, CancellationToken ct = default)
    {
        var r = await _svc.GetByIdAsync(id, ct);
        return r == null ? NotFound() : Ok(r);
    }

    [HttpPut("{id}/active")]
    public async Task<IActionResult> SetActive(string id, [FromQuery] bool active, CancellationToken ct = default)
        => (await _svc.SetActiveAsync(id, active, ct)) ? NoContent() : NotFound();

    [HttpPost("{id}/roles")]
    public async Task<IActionResult> AssignRole(string id, [FromQuery] string role, CancellationToken ct = default)
        => (await _svc.AssignRoleAsync(id, role, ct)) ? NoContent() : NotFound();
}