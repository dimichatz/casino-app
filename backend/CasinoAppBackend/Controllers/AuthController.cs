using System.Security.Claims;
using CasinoAppBackend.DTO;
using CasinoAppBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasinoAppBackend.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IPlayerService _playerService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, IPlayerService playerService,
            ILogger<AuthController> logger)
        {
            _userService = userService;
            _playerService = playerService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<JwtTokenDTO>> Login(UserLoginDTO credentials)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
            var now = DateTime.UtcNow;

            try
            {
                _logger.LogInformation("Login attempt for {Username} from IP {IP} at {Time}.",
                    credentials.UsernameOrEmail, ip, now);

                var user = await _userService.VerifyAndGetUserAsync(credentials);
                var userToken = _userService.CreateUserToken(user);

                _logger.LogInformation("Login SUCCESS for {Username} (UserId {UserId}) from IP {IP} at {Time}.",
                    user.Username, user.Id, ip, now);

                JwtTokenDTO token = new()
                {
                    Token = userToken
                };

                return Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Login FAILED for {Username} from IP {IP} at {Time}.",
                    credentials.UsernameOrEmail, ip, now);

                throw;
            } 
        }

        [HttpPost("signup")]
        public async Task<ActionResult<PlayerSignupResponseDTO>> Signup(PlayerSignUpDTO request)
        {
            var player = await _playerService.SignUpPlayerAsync(request);
            return Ok(player);
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
            var now = DateTime.UtcNow;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.Identity?.Name;

            _logger.LogInformation("Logout for {Username} (UserId {UserId}) from IP {IP} at {Time}.",
                username, userId, ip, now);
            return NoContent();
        }

        [HttpGet("check/email")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            bool UserExists = await _userService.EmailExistsAsync(email);
            return Ok(new {UserExists});
        }

        [HttpGet("check/username")]
        public async Task<IActionResult> CheckUsername(string username)
        {
            bool UserExists = await _userService.UsernameExistsAsync(username);
            return Ok(new {UserExists});
        }

        [HttpGet("check/phone")]
        public async Task<IActionResult> CheckPhone([FromQuery] string phoneNumber)
        {
            bool UserExists = await _userService.PhoneNumberExistsAsync(phoneNumber);
            return Ok(new {UserExists});
        }
    }
}
