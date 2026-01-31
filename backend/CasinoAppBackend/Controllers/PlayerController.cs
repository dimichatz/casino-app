using CasinoAppBackend.Core.Enums;
using CasinoAppBackend.Core.Filters;
using CasinoAppBackend.DTO;
using CasinoAppBackend.DTO.PlayerFullDetailsReadOnlyDTO;
using CasinoAppBackend.DTO.PlayerUpdateFullDetailsPlayerDTO;
using CasinoAppBackend.Extensions;
using CasinoAppBackend.Helpers;
using CasinoAppBackend.Models;
using CasinoAppBackend.Services;
using CasinoAppBackend.Services.Access;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasinoAppBackend.Controllers
{
    [Authorize(Roles = "Player")]
    public class PlayerController : BaseController
    {
        private readonly IPlayerService _playerService;
        private readonly IAccountService _accountService;
        private readonly IPlayerAccessValidator _playerAccessValidator;

        public PlayerController(IPlayerService playerService, IAccountService accountService,
            IPlayerAccessValidator playerAccessValidator)
        {
            _playerService = playerService;
            _accountService = accountService;
            _playerAccessValidator = playerAccessValidator;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<PlayerFullDetailsReadOnlyDTO>> GetProfile()
        {
            var userId = User.GetUserId();
            var playerDto = await _playerService.GetPlayerFullDetailsByUserIdAsync(userId);
            return Ok(playerDto);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> Update([FromBody] PlayerUpdateFullDetailsPlayerDTO request)
        {
            var userId = User.GetUserId();
            await _playerService.UpdatePlayerAsync(userId, request);
            return NoContent();
        }

        [HttpPut("profile/password")]
        public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordDTO request)
        {
            var userId = User.GetUserId();
            await _playerService.ChangePasswordAsync(userId, request);
            return NoContent();
        }

        [HttpGet("transactions")]
        public async Task<ActionResult<PaginatedResult<TransactionReadOnlyDTO>>>
            GetTransactions(int pageNumber, int pageSize, [FromQuery] TransactionFilterDTO filters)
        {
            var userId = User.GetUserId();
            var transactionsDto = await _playerService.GetPaginatedTransactionsByUserIdAsync(userId, pageNumber, pageSize, filters);
            return Ok(transactionsDto);
        }

        [HttpGet("transactions/download")]
        public async Task<ActionResult> DownloadTransactions([FromQuery] TransactionFilterDTO filters)
        {
            var userId = User.GetUserId();
            var transactionsDto = await _playerService.GetTransactionsByUserIdAsync(userId, filters);

            //if (transactionsDto == null || transactionsDto.Count == 0)
            //{
            //    return NoContent();
            //}
            return CsvFileBuilder.ExportCsv(transactionsDto, "transactions");
        }

        [HttpPost("transactions/deposit")]
        public async Task<ActionResult<TransactionReadOnlyDTO>> Deposit([FromBody] TransactionRequestDTO request)
        {
            var userId = User.GetUserId();
            var player = await _playerService.GetPlayerByUserIdAsync(userId);

            request.TransactionType = TransactionType.Deposit;
            request.Currency = "EUR";

            var transactionsDto = await _accountService.ProcessTransactionAsync(player.Id, request);
            return Ok(transactionsDto);
        }

        [HttpPost("transactions/withdraw")]
        public async Task<ActionResult<TransactionReadOnlyDTO>> Withdraw([FromBody] TransactionRequestDTO request)
        {
            var userId = User.GetUserId();
            var player = await _playerService.GetPlayerByUserIdAsync(userId);

            request.TransactionType = TransactionType.Withdraw;
            request.Currency = "EUR";

            var transactionsDto = await _accountService.ProcessTransactionAsync(player.Id, request);
            return Ok(transactionsDto);
        }

        [HttpPost("profile/kyc/upload")]
        public async Task<IActionResult> UploadKycDocument(IFormFile file)
        {
            var userId = User.GetUserId();
            await _playerService.UploadKycAttachmentAsync(userId, file);
            return NoContent();
        }

        [HttpGet("balance")]
        public async Task<ActionResult<PlayerBalanceReadOnlyDTO>> GetBalance()
        {
            var userId = User.GetUserId();
            var player = await _playerService.GetPlayerByUserIdAsync(userId);
            var balanceDto = await _accountService.GetPlayerBalanceAsync(player.Id);
            return Ok(balanceDto);
        }

        [HttpGet("access")]
        public async Task<ActionResult<GameAccessInfoDTO>> GetGameAccess()
        {
            var userId = User.GetUserId();
            var result = await _playerAccessValidator.ValidateAsync(userId);

            return Ok(new GameAccessInfoDTO
            {
                HasGameAccess = result.IsAllowed,
                Reason = result.IsAllowed ? null : result.Failure.ToString()
            });
        }
    }
}
