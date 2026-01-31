using CasinoAppBackend.Core.Enums;
using CasinoAppBackend.Core.Filters;
using CasinoAppBackend.DTO;
using CasinoAppBackend.DTO.AuditReadOnlyDTO;
using CasinoAppBackend.DTO.PlayerFullDetailsReadOnlyDTO;
using CasinoAppBackend.DTO.PlayerUpdateFullDetailsAdminDTO;
using CasinoAppBackend.Helpers;
using CasinoAppBackend.Models;
using CasinoAppBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasinoAppBackend.Controllers.Admin
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    [Route("api/admin/players")]
    public class PlayerManagementController : BaseController
    {
        private readonly IAdminService _adminService;

        public PlayerManagementController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<PlayerReadOnlyDTO>>> GetAll(int pageNumber, int pageSize,
            string? search = null, bool? isActive = null, KycStatus? kycStatus = null)
        {
            var paginatedPlayersDtos = await _adminService.GetPaginatedPlayersFilteredAsync(pageNumber, pageSize,
                search, isActive, kycStatus);
            return Ok(paginatedPlayersDtos);
        }

        [HttpGet("download")]
        public async Task<ActionResult> DownloadPlayers(string? search = null, bool? isActive = null,
            KycStatus? kycStatus = null)
        {
            var playersDtos = await _adminService.GetPlayersFilteredAsync(search, isActive, kycStatus);
            return CsvFileBuilder.ExportCsv(playersDtos, "players");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerFullDetailsReadOnlyDTO>> GetById(Guid id)
        {
            var playerDto = await _adminService.GetPlayerByIdAsync(id);
            return Ok(playerDto);
        }

        [HttpPut("{playerId}")]
        public async Task<IActionResult> Update(Guid playerId, [FromBody] PlayerUpdateFullDetailsAdminDTO request)
        {
            await _adminService.UpdatePlayerAsync(playerId, request, AppUser!.Username!);
            return NoContent();
        }

        [HttpGet("{playerId}/audits/self-exclusions")]
        public async Task<ActionResult<PaginatedResult<PlayerSelfExclusionAuditReadOnlyDTO>>>
            GetPlayerSelfExclusionAudits(Guid playerId, int pageNumber, int pageSize)
        {
            var auditsDtos = await _adminService.GetPaginatedPlayerSelfExclusionAuditsByPlayerIdAsync(playerId, pageNumber, pageSize);
            return Ok(auditsDtos);
        }

        [HttpGet("{playerId}/audits/limits")]
        public async Task<ActionResult<PaginatedResult<PlayerLimitAuditReadOnlyDTO>>>
            GetPlayerLimitAudits(Guid playerId, int pageNumber, int pageSize)
        {
            var auditsDtos = await _adminService.GetPaginatedPlayerLimitAuditsByPlayerIdAsync(playerId, pageNumber, pageSize);
            return Ok(auditsDtos);
        }

        [HttpGet("{playerId}/audits/bans")]
        public async Task<ActionResult<PaginatedResult<PlayerBanAuditReadOnlyDTO>>>
            GetPlayerBanAudits(Guid playerId, int pageNumber, int pageSize)
        {
            var auditsDtos = await _adminService.GetPaginatedPlayerBanAuditsByPlayerIdAsync(playerId, pageNumber, pageSize);
            return Ok(auditsDtos);
        }

        [HttpGet("{playerId}/audits/details")]
        public async Task<ActionResult<PaginatedResult<PlayerDetailsAuditReadOnlyDTO>>>
            GetPlayerDetailsAudits(Guid playerId, int pageNumber, int pageSize)
        {
            var auditsDtos = await _adminService.GetPaginatedPlayerDetailsAuditsByPlayerIdAsync(playerId, pageNumber, pageSize);
            return Ok(auditsDtos);
        }

        [HttpGet("{playerId}/audits/self-exclusions/download")]
        public async Task<ActionResult> DownloadSelfExclusionAudits(Guid playerId)
        {
            var auditsDtos = await _adminService.GetPlayerSelfExclusionAuditsByPlayerIdAsync(playerId);
            return CsvFileBuilder.ExportPlayerAuditCsv(playerId, auditsDtos, "self-exclusions");
        }

        [HttpGet("{playerId}/audits/limits/download")]
        public async Task<ActionResult> DownloadLimitAudits(Guid playerId)
        {
            var auditsDtos = await _adminService.GetPlayerLimitAuditsByPlayerIdAsync(playerId);
            return CsvFileBuilder.ExportPlayerAuditCsv(playerId, auditsDtos, "limits");
        }

        [HttpGet("{playerId}/audits/bans/download")]
        public async Task<ActionResult> DownloadBanAudits(Guid playerId)
        {
            var auditsDtos = await _adminService.GetPlayerBanAuditsByPlayerIdAsync(playerId);
            return CsvFileBuilder.ExportPlayerAuditCsv(playerId, auditsDtos, "bans");
        }

        [HttpGet("{playerId}/audits/details/download")]
        public async Task<ActionResult> DownloadDetailsAudits(Guid playerId)
        {
            var auditsDtos = await _adminService.GetPlayerDetailsAuditsByPlayerIdAsync(playerId);
            return CsvFileBuilder.ExportPlayerAuditCsv(playerId, auditsDtos, "details");
        }

        [HttpGet("{playerId}/transactions")]
        public async Task<ActionResult<PaginatedResult<TransactionReadOnlyDTO>>> 
            GetTransactions(Guid playerId, int pageNumber, int pageSize, [FromQuery] TransactionFilterDTO filters)
        {
            var transactionsDtos = await _adminService.GetPaginatedPlayerTransactionsByPlayerIdAsync(playerId, pageNumber, pageSize, filters);
            return Ok(transactionsDtos);
        }

        [HttpGet("{playerId}/transactions/download")]
        public async Task<ActionResult> DownloadTransactions(Guid playerId, [FromQuery] TransactionFilterDTO filters)
        {
            var transactionsDtos = await _adminService.GetPlayerTransactionsByPlayerIdAsync(playerId, filters);
            return CsvFileBuilder.ExportPlayerAuditCsv(playerId, transactionsDtos, "transactions");
        }

        [HttpGet("{playerId}/attachment")]
        public async Task<ActionResult<AttachmentReadOnlyDTO>> GetAttachment(Guid playerId)
        {
            var attachmentDto = await _adminService.GetPlayerKycAttachmentAsync(playerId);
            return Ok(attachmentDto);
        }
    }
}
