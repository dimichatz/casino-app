using CasinoAppBackend.DTO;
using CasinoAppBackend.Helpers;
using CasinoAppBackend.Models;
using CasinoAppBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasinoAppBackend.Controllers.Admin
{
    [Route("api/admin/admins")]
    public class AdminManagementController : BaseController
    {
        private readonly IAdminService _adminService;

        public AdminManagementController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<PaginatedResult<AdminReadOnlyDTO>>> GetAll(int pageNumber, int pageSize,
            string? search = null, bool? isActive = null)
        {
            var paginatedAdminsDtos = await _adminService.GetPaginatedAdminsFilteredAsync(pageNumber, pageSize, search, isActive);
            return Ok(paginatedAdminsDtos);
        }

        [HttpGet("download")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult> DownloadAdmins(string? search = null, bool? isActive = null)
        {
            var adminsDtos = await _adminService.GetAdminsFilteredAsync(search, isActive);
            return CsvFileBuilder.ExportCsv(adminsDtos, "admins");
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<AdminReadOnlyDTO>> GetById(Guid id)
        {
            var adminDto = await _adminService.GetAdminByIdAsync(id);
            return Ok(adminDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AdminUpdateDTO request)
        {
            await _adminService.UpdateAdminAsync(id, request);
            return NoContent();
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] UserStatusUpdateDTO request)
        {
            await _adminService.ChangeAdminStatusAsync(id, request.IsActive);
            return NoContent();
        }

        [HttpPut("{id}/password")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] UserChangePasswordDTO request)
        {
            await _adminService.ChangePasswordAsync(id, request);
            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create(AdminCreateDTO request)
        {
            var createdAdminDto = await _adminService.CreateAdminAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = createdAdminDto.Id }, createdAdminDto);
        }
    }
}
