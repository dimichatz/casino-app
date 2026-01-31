using CasinoAppBackend.DTO;
using CasinoAppBackend.Models;
using CasinoAppBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasinoAppBackend.Controllers.Admin
{
    [Route("api/admin/games")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class GameManagementController : BaseController
    {
        private readonly IAdminService _adminService;

        public GameManagementController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GameReadOnlyDTO>>> GetAll(int pageNumber, int pageSize,
            string? search = null)
        {
            var paginatedGamesDtos = await _adminService.GetPaginatedGamesFilteredAsync(pageNumber, pageSize, search);
            return Ok(paginatedGamesDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GameReadOnlyDTO>> GetById(Guid id)
        {
            var gameDto = await _adminService.GetGameByIdAsync(id);
            return Ok(gameDto);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] GameStatusUpdateDTO request)
        {
            await _adminService.ChangeGameStatusAsync(id, request.IsEnabled);
            return NoContent();
        }
    }
}
