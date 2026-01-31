using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CasinoAppBackend.Configurations;
using CasinoAppBackend.Data;
using CasinoAppBackend.DTO;
using CasinoAppBackend.Exceptions;
using CasinoAppBackend.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CasinoAppBackend.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;
        private readonly JwtSettings _jwtSettings;

        public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger, IOptions<JwtSettings> jwtOptions)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _jwtSettings = jwtOptions.Value;
        }

        /// <summary>
        /// Authenticates a user by verifying their credentials and ensuring the account is allowed to log in.
        /// </summary>
        /// <param name="credentials">The user's login credentials (username/email and password).</param>
        /// <returns>The authenticated <see cref="User"/> entity.</returns>
        /// <exception cref="EntityNotAuthorizedException">
        /// Thrown when the credentials are invalid or the user account is inactive/banned.
        /// </exception>
        public async Task<User> VerifyAndGetUserAsync(UserLoginDTO credentials)
        {
            User user = await _unitOfWork.UserRepository.GetUserAsync(credentials.UsernameOrEmail!, credentials.Password!)
                    ?? throw new EntityNotAuthorizedException(nameof(User), "BadCredentials");

            if (!user.IsActive)
                throw new EntityNotAuthorizedException(nameof(User), "UserBanned");

            _logger.LogInformation("User with username {Username} found", credentials.UsernameOrEmail);
            return user;
        }

        /// <summary>
        /// Retrieves a user by username.
        /// </summary>
        /// <param name="username">The username to search for.</param>
        /// <returns>The matching <see cref="User"/> entity.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when no user is found with the given username.</exception>
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            User user = await _unitOfWork.UserRepository.GetByUsernameAsync(username)
                    ?? throw new EntityNotFoundException(nameof(User), "User with username: " + username + " not found");

            _logger.LogInformation("User with username: {Username} found", username);
            return user;
        }

        /// <summary>
        /// Retrieves a user by email address.
        /// </summary>
        /// <param name="email">The email to search for.</param>
        /// <returns>The matching <see cref="User"/> entity.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when no user is found with the given email.</exception>
        public async Task<User> GetUserByEmailAsync(string email)
        {
            User user = await _unitOfWork.UserRepository.GetByEmailAsync(email)
                    ?? throw new EntityNotFoundException(nameof(User), "User with email: " + email + " not found");

            _logger.LogInformation("User with email: {Email} found", email);
            return user;
        }

        /// <summary>
        /// Creates a signed JWT token for an authenticated user using the configured JWT settings.
        /// </summary>
        /// <param name="user">The authenticated user entity.</param>
        /// <returns>A signed JWT token as a string.</returns>
        public string CreateUserToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.UserRole.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials
            );

            var userToken = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogInformation("JWT token successfully created for user {Username}", user.Username);

            return userToken;
        }

        /// <summary>
        /// Checks whether the given username exists in the database.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns><c>true</c> if the username exists, <c>false</c> if it doesn't.</returns>
        public async Task<bool> UsernameExistsAsync(string username) =>
            await _unitOfWork.UserRepository.GetByUsernameAsync(username) != null;

        /// <summary>
        /// Checks whether the given email exists in the database.
        /// </summary>
        /// <param name="email">The email to check.</param>
        /// <returns><c>true</c> if the email exists, <c>false</c> if it doesn't.</returns>
        public async Task<bool> EmailExistsAsync(string email) =>
             await _unitOfWork.UserRepository.GetByEmailAsync(email) != null;

        /// <summary>
        /// Checks whether the given phone number exists in the database.
        /// </summary>
        /// <param name="phoneNumber">The phone number to check.</param>
        /// <returns><c>true</c> if the phone number exists, <c>false</c> if it doesn't.</returns>
        public async Task<bool> PhoneNumberExistsAsync(string phoneNumber) =>
             await _unitOfWork.UserRepository.GetByPhoneNumberAsync(phoneNumber) != null;
    }
}