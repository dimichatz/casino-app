using CasinoAppBackend.DTO;
using CasinoAppBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasinoAppBackend.Controllers.Admin
{
    [Authorize(Roles = "SuperAdmin")]
    [Route("api/admin/appsettings")]
    public class AppSettingsManagementController : BaseController
    {
        private readonly IAdminService _adminService;

        public AppSettingsManagementController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<ActionResult<AppSettingsReadOnlyDTO>> GetAppSettings()
        {
            var appSettingsDto = await _adminService.GetAppSettingsAsync();
            return Ok(appSettingsDto);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAppSettings([FromBody] AppSettingsUpdateDTO request)
        {
            await _adminService.UpdateAppSettingsAsync(request);
            return NoContent();
        }
    }
}
