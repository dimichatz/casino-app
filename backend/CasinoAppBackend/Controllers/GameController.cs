using CasinoAppBackend.DTO;
using CasinoAppBackend.Models;
using CasinoAppBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasinoAppBackend.Controllers
{
    [Route("api/games")]
    //[Authorize(Roles = "Player")]
    public class GameController : BaseController
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GameReadOnlyDTO>>> GetAll(int pageNumber, int pageSize,
            string? search = null)
        {
            var paginatedGamesDtos = await _gameService.GetPaginatedGamesFilteredAsync(pageNumber, pageSize, search);
            return Ok(paginatedGamesDtos);
        }
    }
}
