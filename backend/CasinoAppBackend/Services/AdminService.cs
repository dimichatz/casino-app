using System.Text.Json;
using System.Text.RegularExpressions;
using AutoMapper;
using CasinoAppBackend.Core.Enums;
using CasinoAppBackend.Core.Filters;
using CasinoAppBackend.Data;
using CasinoAppBackend.DTO;
using CasinoAppBackend.DTO.AuditReadOnlyDTO;
using CasinoAppBackend.DTO.PlayerFullDetailsReadOnlyDTO;
using CasinoAppBackend.DTO.PlayerUpdateFullDetailsAdminDTO;
using CasinoAppBackend.Exceptions;
using CasinoAppBackend.Helpers;
using CasinoAppBackend.Models;
using CasinoAppBackend.Repositories;
using CasinoAppBackend.Security;
using CasinoAppBackend.Services.FileStorage;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CasinoAppBackend.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminService> _logger;
        private readonly IAccountService _accountService;
        private readonly IFileStorageService _fileStorageService;

        public AdminService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AdminService> logger,
            IAccountService accountService, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _accountService = accountService;
            _fileStorageService = fileStorageService;
        }

        /// <summary>
        /// Creates a new admin user after validating that the username, email, and phone number are unique.
        /// If any conflicts are found, the appropriate exception is thrown.
        /// </summary>
        /// <param name="request">The DTO containing the new admin's details.</param>
        /// <exception cref="EntityAlreadyExistsException">
        /// Thrown when an existing user is found with the same username, email, or phone number.
        /// </exception>
        /// <exception cref="MultipleErrorsException">
        /// Thrown when multiple validation conflicts are detected while creating the admin.
        /// </exception>
        public async Task<AdminReadOnlyDTO> CreateAdminAsync(AdminCreateDTO request)
        {
            var user = _mapper.Map<User>(request);
            var errors = new List<ErrorDetail>();

            if (await _unitOfWork.UserRepository.GetByUsernameAsync(user.Username) is not null)
                errors.Add(new ErrorDetail(nameof(EntityAlreadyExistsException), "UserAlreadyExists",
                    "User with username " + user.Username + " already exists."));

            if (await _unitOfWork.UserRepository.GetByEmailAsync(user.Email) is not null)
                errors.Add(new ErrorDetail(nameof(EntityAlreadyExistsException), "UserAlreadyExists",
                    "User with email " + user.Email + " already exists."));

            if (await _unitOfWork.UserRepository.GetByPhoneNumberAsync(user.PhoneNumber) is not null)
                errors.Add(new ErrorDetail(nameof(EntityAlreadyExistsException), "UserAlreadyExists",
                    "User with phone number " + user.PhoneNumber + " already exists."));

            if (errors.Count > 0)
            {
                if (errors.Count == 1)
                {
                    var error = errors[0];
                    throw new EntityAlreadyExistsException(error.Code!, error.Message!);
                }
                throw new MultipleErrorsException(errors, "CreateAdminMultipleErrors",
                                "Multiple errors occurred while creating the user.");
            }

            user.UserRole = UserRole.Admin;
            user.Password = EncryptionUtil.Encrypt(user.Password);
            await _unitOfWork.UserRepository.AddAsync(user);

            var admin = _mapper.Map<Admin>(request);
            admin.User = user;
            await _unitOfWork.AdminRepository.AddAsync(admin);

            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Admin {Username} created successfully.", user.Username);
            return _mapper.Map<AdminReadOnlyDTO>(admin);
        }

        /// <summary>
        /// Updates an existing admin information after validating that the new email and phone number (if provided) are unique.
        /// Throws appropriate exceptions if conflicts are detected or the admin does not exist.
        /// </summary>
        /// <param name="request">The DTO containing the updated admin details.</param>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when the specified admin or its associated user record cannot be found.
        /// </exception>
        /// <exception cref="EntityAlreadyExistsException">
        /// Thrown when the new email or phone number is already associated with another user.
        /// </exception>
        /// <exception cref="MultipleErrorsException">
        /// Thrown when multiple uniqueness conflicts occur during validation.
        /// </exception>
        public async Task UpdateAdminAsync(Guid id, AdminUpdateDTO request)
        {
            var errors = new List<ErrorDetail>();
            var existingAdmin = await _unitOfWork.AdminRepository.GetAsync(id)
                ?? throw new EntityNotFoundException(nameof(Admin), "Admin with id: " + id + " not found.");
            var existingUser = await _unitOfWork.UserRepository.GetAsync(existingAdmin.UserId)
                ?? throw new EntityNotFoundException(nameof(User), "User with id: " + existingAdmin.UserId + " not found.");

            if (request.Email is not null && existingUser.Email != request.Email &&
                await _unitOfWork.UserRepository.GetByEmailAsync(request.Email) is not null)
                errors.Add(new ErrorDetail(nameof(EntityAlreadyExistsException), "UserAlreadyExists",
                    "User with email " + request.Email + " already exists."));

            if (request.PhoneNumber is not null && existingUser.PhoneNumber != request.PhoneNumber &&
                await _unitOfWork.UserRepository.GetByPhoneNumberAsync(request.PhoneNumber) is not null)
                errors.Add(new ErrorDetail(nameof(EntityAlreadyExistsException), "UserAlreadyExists",
                    "User with phone number " + request.PhoneNumber + " already exists."));

            if (errors.Count > 0)
            {
                if (errors.Count == 1)
                {
                    var error = errors[0];
                    throw new EntityAlreadyExistsException(error.Code!, error.Message!);
                }
                throw new MultipleErrorsException(errors, "UpdateAdminMultipleErrors",
                                "Multiple errors occurred while updating the user.");
            }

            _mapper.Map(request, existingUser);
            _mapper.Map(request, existingAdmin);

            await _unitOfWork.SaveAsync();

            _logger!.LogInformation("Admin {Username} updated successfully.", existingUser.Username);
        }

        /// <summary>
        /// Changes the password of an existing admin user after validating the current password
        /// and ensuring the new password is not the same as the old one.
        /// </summary>
        /// /// <param name="id">The ID of the admin whose password is being changed.</param>
        /// <param name="request">The DTO containing the current and new password values.</param>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when the admin with the specified ID or the associated user does not exist.
        /// </exception>
        /// <exception cref="DomainValidationException">
        /// Thrown when the current password is incorrect or when the new password matches the current one.
        /// </exception>
        public async Task ChangePasswordAsync(Guid id, UserChangePasswordDTO request)
        {
            var admin = await _unitOfWork.AdminRepository.GetAsync(id)
                ?? throw new EntityNotFoundException(nameof(Admin), "Admin with id: " + id + " not found.");
            var user = await _unitOfWork.UserRepository.GetAsync(admin.UserId)
                ?? throw new EntityNotFoundException(nameof(User), "User with id: " + admin.UserId + " not found.");

            if (!EncryptionUtil.IsValidPassword(request.CurrentPassword!, user.Password))
                throw new DomainValidationException("InvalidPassword", "Current password is incorrect.");
            if (request.CurrentPassword == request.NewPassword)
                throw new DomainValidationException("PasswordReuseNotAllowed", "New password must not be the same as current password.");

            user.Password = EncryptionUtil.Encrypt(request.NewPassword!);
            await _unitOfWork.SaveAsync();
        }

        /// <summary>
        /// Retrieves a single admin by its ID.
        /// </summary>
        /// <param name="id">The ID of the admin to retrieve.</param>
        /// <returns>
        /// An <see cref="AdminReadOnlyDTO"/> containing the admin’s details, or null if not found.
        /// </returns>
        /// /// <exception cref="EntityNotFoundException">
        /// Thrown when no admin exists with the specified ID.
        /// </exception>
        public async Task<AdminReadOnlyDTO> GetAdminByIdAsync(Guid id)
        {
            var admin = await _unitOfWork.AdminRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException(nameof(Admin), "Admin with id: " + id + " not found");
            return _mapper.Map<AdminReadOnlyDTO>(admin);
        }

        /// <summary>
        /// Retrieves a filtered list of admins based on search criteria and active status.
        /// </summary>
        /// <param name="search">An optional search string to filter admins by name, username, or email.</param>
        /// <param name="isActive">An optional flag to filter active or inactive admins.</param>
        /// <returns>
        /// A list of <see cref="AdminReadOnlyDTO"/> objects matching the specified filters.
        /// </returns>
        public async Task<List<AdminReadOnlyDTO>> GetAdminsFilteredAsync(string? search, bool? isActive)
        {
            var admins = await _unitOfWork.AdminRepository.GetAdminsFilteredAsync(search, isActive);
            return _mapper.Map<List<AdminReadOnlyDTO>>(admins);
        }

        /// <summary>
        /// Retrieves a paginated and filtered list of admins based on search criteria and active status.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of records per page.</param>
        /// <param name="search">An optional search string to filter admins by name, username, or email.</param>
        /// <param name="isActive">An optional flag to filter active or inactive admins.</param>
        /// <returns>
        /// A <see cref="PaginatedResult{AdminReadOnlyDTO}"/> containing the filtered and paginated admin list.
        /// </returns>
        public async Task<PaginatedResult<AdminReadOnlyDTO>> GetPaginatedAdminsFilteredAsync(int pageNumber, int pageSize,
            string? search, bool? isActive)
        {
            var paginatedAdmins = await _unitOfWork.AdminRepository
                .GetPaginatedAdminsFilteredAsync(pageNumber, pageSize, search, isActive);

            var adminDtos = _mapper.Map<List<AdminReadOnlyDTO>>(paginatedAdmins.Data);

            return new PaginatedResult<AdminReadOnlyDTO>
            {
                Data = adminDtos,
                TotalRecords = paginatedAdmins.TotalRecords,
                PageNumber = paginatedAdmins.PageNumber,
                PageSize = paginatedAdmins.PageSize
            };
        }

        /// <summary>
        /// Retrieves the application settings and returns them as a strongly-typed DTO.
        /// </summary>
        /// <returns>
        /// An <see cref="AppSettingsReadOnlyDTO"/> containing all stored application settings.
        /// </returns>
        /// <remarks>
        /// This method expects the settings table to contain all required keys. Missing or invalid values
        /// will result in exceptions during parsing.
        /// </remarks>
        public async Task<AppSettingsReadOnlyDTO> GetAppSettingsAsync()
        {
            var appSettings = await _unitOfWork.AppSettingRepository.GetAllAsync();
            var settingsDictionary = appSettings.ToDictionary(s => s.Key, s => s.Value);

            return new AppSettingsReadOnlyDTO
            {
                DefaultSignupBonus = decimal.Parse(settingsDictionary["DefaultSignupBonus"]),
                DepositDailyLimit = int.Parse(settingsDictionary["DepositDailyLimit"]),
                DepositWeeklyLimit = int.Parse(settingsDictionary["DepositWeeklyLimit"]),
                DepositMonthlyLimit = int.Parse(settingsDictionary["DepositMonthlyLimit"]),
                LossDailyLimit = int.Parse(settingsDictionary["LossDailyLimit"]),
                LossWeeklyLimit = int.Parse(settingsDictionary["LossWeeklyLimit"]),
                LossMonthlyLimit = int.Parse(settingsDictionary["LossMonthlyLimit"]),
                LimitIncreaseDelayDays = int.Parse(settingsDictionary["LimitIncreaseDelayDays"])
            };
        }

        /// <summary>
        /// Updates all configurable application settings such as signup bonus, deposit limits,
        /// loss limits, and limit increase delay. 
        /// Creates missing settings and updates only those whose values have changed.
        /// </summary>
        /// <param name="request">
        /// A DTO containing the complete set of validated application configuration values.
        /// </param>
        public async Task UpdateAppSettingsAsync(AppSettingsUpdateDTO request)
        {
            var appSettingsDictionary = new Dictionary<string, string>
            {
                { "DefaultSignupBonus", request.DefaultSignupBonus.ToString() },
                { "DepositDailyLimit", request.DepositDailyLimit.ToString() },
                { "DepositWeeklyLimit", request.DepositWeeklyLimit.ToString() },
                { "DepositMonthlyLimit", request.DepositMonthlyLimit.ToString() },
                { "LossDailyLimit", request.LossDailyLimit.ToString() },
                { "LossWeeklyLimit", request.LossWeeklyLimit.ToString() },
                { "LossMonthlyLimit", request.LossMonthlyLimit.ToString() },
                { "LimitIncreaseDelayDays", request.LimitIncreaseDelayDays.ToString() }
            };

            foreach (var appSetting in appSettingsDictionary)
            {
                var setting = await _unitOfWork.AppSettingRepository.GetByKeyAsync(appSetting.Key);

                if (setting is null)
                {
                    await _unitOfWork.AppSettingRepository.AddAsync(new AppSetting
                    {
                        Key = appSetting.Key,
                        Value = appSetting.Value
                    });
                    _logger.LogInformation("New app setting {Key} created successfully", appSetting.Key);
                }
                else if (setting.Value != appSetting.Value)
                {
                    setting.Value = appSetting.Value;
                    await _unitOfWork.AppSettingRepository.UpdateAsync(setting);
                    _logger.LogInformation("App setting {Key} updated successfully", appSetting.Key);
                }
            }
            await _unitOfWork.SaveAsync();
        }

        /// <summary>
        /// Changes the active status of an admin by updating the associated user account.
        /// </summary>
        /// <param name="id">The ID of the admin whose status is being updated.</param>
        /// <param name="isActive">The new active status to assign to the user.</param>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when the admin with the specified ID or the associated user record does not exist.
        /// </exception>
        public async Task ChangeAdminStatusAsync(Guid id, bool isActive)
        {
            var admin = await _unitOfWork.AdminRepository.GetAsync(id)
                ?? throw new EntityNotFoundException(nameof(Admin), "Admin with id: " + id + " not found");
            var user = await _unitOfWork.UserRepository.GetAsync(admin.UserId)
                ?? throw new EntityNotFoundException(nameof(User), "User with id: " + admin.UserId + " not found");

            user.IsActive = isActive;
            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Admin {Id} status changed sucessfully.", id);
        }

        /// <summary>
        /// Updates a player's full profile details, applying only the sections of data provided in the request.
        /// Supports partial updates across user information, address, KYC details, and account status.
        /// All field-level changes are captured through audit logs for administrative traceability.
        /// </summary>
        /// <param name="playerId">The ID of the player to update.</param>
        /// <param name="request">
        /// The DTO containing the section (user, address, KYC, or status) of player's data to update. 
        /// </param>
        /// <param name="username">The username of the admin performing the update, used for audit tracking.</param>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when the player, associated user, KYC document, or performing admin cannot be found.
        /// </exception>
        /// <exception cref="EntityAlreadyExistsException">
        /// Thrown when the updated email or phone number already exists on another user account.
        /// </exception>
        /// <exception cref="MultipleErrorsException">
        /// Thrown when multiple uniqueness validation conflicts are detected during the update.
        /// </exception>
        /// <remarks>
        /// This method:
        /// <list type="bullet">
        ///<item>Creates deep copies of the current<see cref="User"/>, <see cref = "Player" />,and<see cref = "KycDocument" /> entities
        ///to accurately capture their original state before any modifications.</item>
        /// <item>Validates uniqueness of critical identifiers such as email and phone number.</item>
        /// <item>Applies changes only to provided data segments (user, address, KYC, or status) without affecting untouched fields.</item>
        /// <item>Generates detailed <see cref="PlayerDetailsAudit"/> records for each modified field
        /// across the <see cref="User"/>, <see cref="Player"/>, and <see cref="KycDocument"/> entities, 
        /// except when only the user status is updated.</item>
        /// <item>Creates a dedicated player ban or reactivation audit entry through <see cref="CreatePlayerBanAuditAsync"/> 
        /// whenever the account’s active status is changed.</item>
        /// <item>Commits all updates and audit entries atomically to ensure consistency and traceability.</item>
        /// </list>
        /// </remarks>
        public async Task UpdatePlayerAsync(Guid playerId, PlayerUpdateFullDetailsAdminDTO request, string username)
        {
            var errors = new List<ErrorDetail>();
            var existingPlayer = await _unitOfWork.PlayerRepository.GetAsync(playerId)
                ?? throw new EntityNotFoundException(nameof(Player), "Player with id: " + playerId + " not found.");
            var existingUser = await _unitOfWork.UserRepository.GetAsync(existingPlayer.UserId)
                ?? throw new EntityNotFoundException(nameof(User), "User with id: " + existingPlayer.UserId + " not found.");
            var existingKycDocument = await _unitOfWork.KycDocumentRepository.GetByPlayerIdAsync(playerId)
                ?? throw new EntityNotFoundException(nameof(KycDocument), "KYC Document for player with id: " + playerId + " not found.");
            var adminUser = await _unitOfWork.UserRepository.GetByUsernameAsync(username)
                ?? throw new EntityNotFoundException(nameof(User), "User with username: " + username + " not found.");

            // deep copy of old values
            var allPlayerDetailsAudits = new List<PlayerDetailsAudit>();
            var oldPlayer = await _unitOfWork.PlayerRepository.Query().AsNoTracking().FirstAsync(p => p.Id == playerId);
            var oldUser = await _unitOfWork.UserRepository.Query().AsNoTracking().FirstAsync(u => u.Id == existingPlayer.UserId);
            var oldKycDocument = await _unitOfWork.KycDocumentRepository.Query().AsNoTracking().FirstAsync(k => k.PlayerId == playerId);

            var oldIsActive = existingUser.IsActive;
            if (request.UserStatusDetails?.IsActive is not null && oldIsActive != request.UserStatusDetails.IsActive)
            {
                _mapper.Map(request.UserStatusDetails, existingUser);
                await CreatePlayerBanAuditAsync(existingUser, adminUser, request.Comment, playerId);
            }

            if (request.UserDetails is not null)
            {
                if (request.UserDetails.Email is not null
                        && existingUser.Email != request.UserDetails.Email
                        && await _unitOfWork.UserRepository.GetByEmailAsync(request.UserDetails.Email) is not null)
                    errors.Add(new ErrorDetail(nameof(EntityAlreadyExistsException), "UserAlreadyExists",
                        "User with email " + request.UserDetails.Email + " already exists."));
                if (request.UserDetails.PhoneNumber is not null
                        && existingUser.PhoneNumber != request.UserDetails.PhoneNumber
                        && await _unitOfWork.UserRepository.GetByPhoneNumberAsync(request.UserDetails.PhoneNumber) is not null)
                    errors.Add(new ErrorDetail(nameof(EntityAlreadyExistsException), "PlayerAlreadyExists",
                        "User with phone number " + request.UserDetails.PhoneNumber + " already exists."));

                if (errors.Count > 0)
                {
                    if (errors.Count == 1)
                    {
                        var error = errors.First();
                        throw new EntityAlreadyExistsException(error.Code!, error.Message!);
                    }
                    throw new MultipleErrorsException(errors, "UpdatePlayerMultipleErrors",
                                    "Multiple errors occurred while updating the player.");
                }

                _mapper.Map(request.UserDetails, existingUser);
                var userAudits = EntityAuditBuilder.GetChangedFields(oldUser, existingUser, adminUser, playerId, request.Comment);
                allPlayerDetailsAudits.AddRange(userAudits);
            }

            if (request.AddressDetails is not null)
            {
                _mapper.Map(request.AddressDetails, existingPlayer);
                var playerAudits = EntityAuditBuilder.GetChangedFields(oldPlayer, existingPlayer, adminUser, playerId, request.Comment);
                allPlayerDetailsAudits.AddRange(playerAudits);
            }

            if (request.KycDetails is not null)
            {
                _mapper.Map(request.KycDetails, existingPlayer);
                _mapper.Map(request.KycDetails, existingUser);
                _mapper.Map(request.KycDetails, existingKycDocument);
                existingKycDocument.KycCheckDate = DateTime.UtcNow;
                existingKycDocument.KycCheckedBy = adminUser.Username;

                var userAudits = EntityAuditBuilder.GetChangedFields(oldUser, existingUser, adminUser,
                    playerId, request.Comment);
                var kycDocumentAudits = EntityAuditBuilder.GetChangedFields(oldKycDocument, existingKycDocument, adminUser,
                    playerId, request.Comment);
                allPlayerDetailsAudits.AddRange(userAudits);
                allPlayerDetailsAudits.AddRange(kycDocumentAudits);
            }

            if (allPlayerDetailsAudits.Count > 0)
                await _unitOfWork.PlayerDetailsAuditRepository.AddRangeAsync(allPlayerDetailsAudits);
            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Player {Username} updated successfully.", existingUser.Username);
        }

        /// <summary>
        /// Retrieves a player’s full profile details by their ID.
        /// </summary>
        /// <param name="id">The ID of the player to retrieve.</param>
        /// <returns>
        /// A <see cref="PlayerFullDetailsReadOnlyDTO"/> containing the complete player information, 
        /// including user, address, and KYC details.
        /// </returns>
        public async Task<PlayerFullDetailsReadOnlyDTO> GetPlayerByIdAsync(Guid id)
        {
            var player = await _unitOfWork.PlayerRepository.GetFullDetailsByIdAsync(id);
            return _mapper.Map<PlayerFullDetailsReadOnlyDTO>(player);
        }

        /// <summary>
        /// Retrieves a filtered list of players based on search criteria, account activity, and KYC status.
        /// </summary>
        /// <param name="search">An optional search term to filter players by username, or email.</param>
        /// <param name="isActive">An optional flag to filter active or inactive player accounts.</param>
        /// <param name="kycStatus">An optional filter to match players by their current KYC verification status.</param>
        /// <returns>
        /// A list of <see cref="PlayerReadOnlyDTO"/> objects matching the specified filters.
        /// </returns>
        public async Task<List<PlayerReadOnlyDTO>> GetPlayersFilteredAsync(string? search, bool? isActive, KycStatus? kycStatus)
        {
            var players = await _unitOfWork.PlayerRepository.GetPlayersFilteredAsync(search, isActive, kycStatus);
            return _mapper.Map<List<PlayerReadOnlyDTO>>(players);
        }

        /// <summary>
        /// Retrieves a paginated list of players filtered by search criteria, activity status, and KYC verification state.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of records per page.</param>
        /// <param name="search">An optional search term to filter players by username, or email.</param>
        /// <param name="isActive">An optional flag to filter active or inactive player accounts.</param>
        /// <param name="kycStatus">An optional filter to match players by their KYC verification status.</param>
        /// <returns>
        /// A <see cref="PaginatedResult{PlayerReadOnlyDTO}"/> containing the filtered and paginated player list.
        /// </returns>
        public async Task<PaginatedResult<PlayerReadOnlyDTO>> GetPaginatedPlayersFilteredAsync(int pageNumber, int pageSize,
            string? search, bool? isActive, KycStatus? kycStatus)
        {
            var paginatedPlayers = await _unitOfWork.PlayerRepository
                .GetPaginatedPlayersFilteredAsync(pageNumber, pageSize, search, isActive, kycStatus);

            var playerDtos = _mapper.Map<List<PlayerReadOnlyDTO>>(paginatedPlayers.Data);

            return new PaginatedResult<PlayerReadOnlyDTO>
            {
                Data = playerDtos,
                TotalRecords = paginatedPlayers.TotalRecords,
                PageNumber = paginatedPlayers.PageNumber,
                PageSize = paginatedPlayers.PageSize
            };
        }

        /// <summary>
        /// Creates an audit entry when a player's account is banned or reactivated by an admin.
        /// </summary>
        /// <param name="existingUser">The user entity representing the player whose status changed.</param>
        /// <param name="adminUser">The admin performing the action.</param>
        /// <param name="comment">An optional comment describing the reason for the action.</param>
        /// <param name="playerId">The ID of the affected player.</param>
        /// <remarks>
        /// This method is called automatically when a player's active status is modified in 
        /// <see cref="UpdatePlayerAsync"/>. It records whether the player was banned or reactivated, 
        /// along with the responsible admin and optional comment.
        /// </remarks>
        private async Task CreatePlayerBanAuditAsync(User existingUser, User adminUser, string? comment, Guid playerId)
        {
            var audit = new PlayerBanAudit
            {
                ChangedByUserId = adminUser.Id,
                ChangedByUsername = adminUser.Username,
                IsBanned = !existingUser.IsActive,
                Comment = comment,
                PlayerId = playerId
            };
            await _unitOfWork.PlayerBanAuditRepository.AddAsync(audit);
        }

        /// <summary>
        /// Retrieves all self-exclusion audit records for a specific player.
        /// </summary>
        /// <param name="playerId">The ID of the player.</param>
        /// <returns>
        /// A list of <see cref="PlayerSelfExclusionAuditReadOnlyDTO"/> containing self-exclusion audit records.
        /// </returns>
        public async Task<List<PlayerSelfExclusionAuditReadOnlyDTO>> GetPlayerSelfExclusionAuditsByPlayerIdAsync(Guid playerId)
        {
            var audits = await _unitOfWork.PlayerSelfExclusionAuditRepository.GetAuditsByPlayerIdAsync(playerId);
            return _mapper.Map<List<PlayerSelfExclusionAuditReadOnlyDTO>>(audits);
        }

        /// <summary>
        /// Retrieves all limit change audit records for a specific player.
        /// </summary>
        /// <param name="playerId">The ID of the player.</param>
        /// <returns>
        /// A list of <see cref="PlayerLimitAuditReadOnlyDTO"/> containing player limit audit records.
        /// </returns>
        public async Task<List<PlayerLimitAuditReadOnlyDTO>> GetPlayerLimitAuditsByPlayerIdAsync(Guid playerId)
        {
            var audits = await _unitOfWork.PlayerLimitAuditRepository.GetAuditsByPlayerIdAsync(playerId);
            return _mapper.Map<List<PlayerLimitAuditReadOnlyDTO>>(audits);
        }

        /// <summary>
        /// Retrieves all detail change audit records for a specific player.
        /// </summary>
        /// <param name="playerId">The ID of the player.</param>
        /// <returns>
        /// A list of <see cref="PlayerDetailsAuditReadOnlyDTO"/> containing player details audit records.
        /// </returns>
        public async Task<List<PlayerDetailsAuditReadOnlyDTO>> GetPlayerDetailsAuditsByPlayerIdAsync(Guid playerId)
        {
            var audits = await _unitOfWork.PlayerDetailsAuditRepository.GetAuditsByPlayerIdAsync(playerId);
            return _mapper.Map<List<PlayerDetailsAuditReadOnlyDTO>>(audits);
        }

        /// <summary>
        /// Retrieves all account ban or reactivation audit records for a specific player.
        /// </summary>
        /// <param name="playerId">The ID of the player.</param>
        /// <returns>
        /// A list of <see cref="PlayerBanAuditReadOnlyDTO"/> containing all player ban audit details.
        /// </returns>
        public async Task<List<PlayerBanAuditReadOnlyDTO>> GetPlayerBanAuditsByPlayerIdAsync(Guid playerId)
        {
            var audits = await _unitOfWork.PlayerBanAuditRepository.GetAuditsByPlayerIdAsync(playerId);
            return _mapper.Map<List<PlayerBanAuditReadOnlyDTO>>(audits);
        }

        /// <summary>
        /// Retrieves a paginated list of self-exclusion audit records for a specific player.
        /// </summary>
        /// <param name="playerId">The ID of the player.</param>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of records per page.</param>

        /// <returns>
        /// A <see cref="PaginatedResult{PlayerSelfExclusionAuditReadOnlyDTO}"/> containing self-exclusion audit records.
        /// </returns>
        public async Task<PaginatedResult<PlayerSelfExclusionAuditReadOnlyDTO>> GetPaginatedPlayerSelfExclusionAuditsByPlayerIdAsync(Guid playerId,
            int pageNumber, int pageSize)
        {
            var paginatedAudits = await _unitOfWork.PlayerSelfExclusionAuditRepository
              .GetPaginatedAuditsByPlayerIdAsync(playerId, pageNumber, pageSize);

            var auditDtos = _mapper.Map<List<PlayerSelfExclusionAuditReadOnlyDTO>>(paginatedAudits.Data);

            return new PaginatedResult<PlayerSelfExclusionAuditReadOnlyDTO>
            {
                Data = auditDtos,
                TotalRecords = paginatedAudits.TotalRecords,
                PageNumber = paginatedAudits.PageNumber,
                PageSize = paginatedAudits.PageSize
            };
        }

        /// <summary>
        /// Retrieves a paginated list of limit change audit records for a specific player.
        /// </summary>
        /// <param name="playerId">The ID of the player.</param>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of records per page.</param>
        /// <returns>
        /// A <see cref="PaginatedResult{PlayerLimitAuditReadOnlyDTO}"/> containing player limit audit records.
        /// </returns>
        public async Task<PaginatedResult<PlayerLimitAuditReadOnlyDTO>> GetPaginatedPlayerLimitAuditsByPlayerIdAsync(Guid playerId,
            int pageNumber, int pageSize)
        {
            var paginatedAudits = await _unitOfWork.PlayerLimitAuditRepository
              .GetPaginatedAuditsByPlayerIdAsync(playerId, pageNumber, pageSize);

            var auditDtos = _mapper.Map<List<PlayerLimitAuditReadOnlyDTO>>(paginatedAudits.Data);

            return new PaginatedResult<PlayerLimitAuditReadOnlyDTO>
            {
                Data = auditDtos,
                TotalRecords = paginatedAudits.TotalRecords,
                PageNumber = paginatedAudits.PageNumber,
                PageSize = paginatedAudits.PageSize
            };
        }

        /// <summary>
        /// Retrieves a paginated list of player details audit records for a specific player.
        /// </summary>
        /// <param name="playerId">The ID of the player.</param>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of records per page.</param>
        /// <returns>
        /// A <see cref="PaginatedResult{PlayerDetailsAuditReadOnlyDTO}"/> containing player details audit records.
        /// </returns>
        public async Task<PaginatedResult<PlayerDetailsAuditReadOnlyDTO>> GetPaginatedPlayerDetailsAuditsByPlayerIdAsync(Guid playerId,
            int pageNumber, int pageSize)
        {
            var paginatedAudits = await _unitOfWork.PlayerDetailsAuditRepository
              .GetPaginatedAuditsByPlayerIdAsync(playerId, pageNumber, pageSize);

            var auditDtos = _mapper.Map<List<PlayerDetailsAuditReadOnlyDTO>>(paginatedAudits.Data);

            return new PaginatedResult<PlayerDetailsAuditReadOnlyDTO>
            {
                Data = auditDtos,
                TotalRecords = paginatedAudits.TotalRecords,
                PageNumber = paginatedAudits.PageNumber,
                PageSize = paginatedAudits.PageSize
            };
        }

        /// <summary>
        /// Retrieves a paginated list of ban or reactivation audit records for a specific player.
        /// </summary>
        /// <param name="playerId">The ID of the player.</param>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of records per page.</param>
        /// <returns>
        /// A <see cref="PaginatedResult{PlayerBanAuditReadOnlyDTO}"/> containing all player ban audit details.
        /// </returns>
        public async Task<PaginatedResult<PlayerBanAuditReadOnlyDTO>> GetPaginatedPlayerBanAuditsByPlayerIdAsync(Guid playerId,
            int pageNumber, int pageSize)
        {
            var paginatedAudits = await _unitOfWork.PlayerBanAuditRepository
              .GetPaginatedAuditsByPlayerIdAsync(playerId, pageNumber, pageSize);

            var auditDtos = _mapper.Map<List<PlayerBanAuditReadOnlyDTO>>(paginatedAudits.Data);

            return new PaginatedResult<PlayerBanAuditReadOnlyDTO>
            {
                Data = auditDtos,
                TotalRecords = paginatedAudits.TotalRecords,
                PageNumber = paginatedAudits.PageNumber,
                PageSize = paginatedAudits.PageSize
            };
        }

        /// <summary>
        /// Retrieves all account transactions for a specific player, applying optional filters
        /// such as transaction number, date range, and transaction type.
        /// </summary>
        /// <param name="playerId">
        /// The ID (<see cref="Guid"/>) of the player whose transactions are being retrieved.
        /// </param>
        /// <param name="filters">
        /// The filter object containing optional criteria (transaction number, date range,
        /// and transaction type categories) used to narrow down the returned transactions.
        /// </param>
        /// <returns>
        /// A list of <see cref="TransactionReadOnlyDTO"/> representing the player's filtered transactions.
        /// </returns>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when the account associated with the specified player ID cannot be found.
        /// </exception>
        public async Task<List<TransactionReadOnlyDTO>> GetPlayerTransactionsByPlayerIdAsync(Guid playerId, TransactionFilterDTO filters)
        {
            var account = await _unitOfWork.AccountRepository.GetByPlayerIdAsync(playerId)
                ?? throw new EntityNotFoundException(nameof(Account), "Account with player id: " + playerId + " not found.");

            return await _accountService.GetAllTransactionsByAccountIdFilteredAsync(account.Id, filters);
        }

        /// <summary>
        /// Retrieves paginated account transactions for a specific player, applying optional filters such as
        /// transaction number, date range, and transaction type.
        /// </summary>
        /// <param name="playerId">The ID (<see cref="Guid"/>) of the player whose transactions are being retrieved.</param>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The number of records returned per page.</param>
        /// <param name="filters">
        /// An object containing optional criteria (transaction number, date range,
        /// and transaction type categories) used to refine the returned transactions.
        /// </param>
        /// <returns>
        /// A <see cref="PaginatedResult{TransactionReadOnlyDTO}"/> containing the filtered transactions
        /// together with pagination metadata.
        /// </returns>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when no account is found for the specified player ID.
        /// </exception>
        public async Task<PaginatedResult<TransactionReadOnlyDTO>> GetPaginatedPlayerTransactionsByPlayerIdAsync(Guid playerId,
            int pageNumber, int pageSize, TransactionFilterDTO filters)
        {
            var account = await _unitOfWork.AccountRepository.GetByPlayerIdAsync(playerId)
                ?? throw new EntityNotFoundException(nameof(Account), "Account with player id: " + playerId + " not found.");

            return await _accountService.GetPaginatedTransactionsByAccountIdFilteredAsync(account.Id, pageNumber, pageSize,
                filters);
        }

        /// <summary>
        /// Retrieves the player's uploaded KYC attachment and generates a temporary
        /// SAS download URL that allows to securely view or download the file.
        /// </summary>
        /// <param name="playerId">
        /// The ID of the player whose KYC attachment is being requested.
        /// </param>
        /// <returns>
        /// An <see cref="AttachmentReadOnlyDTO"/> containing file metadata and a time-limited
        /// download URL for reviewing the attachment.
        /// </returns>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when either the player's <see cref="KycDocument"/> or its associated
        /// <see cref="Attachment"/> cannot be found.
        /// </exception>
        public async Task<AttachmentReadOnlyDTO> GetPlayerKycAttachmentAsync(Guid playerId)
        {
            var kycDocument = await _unitOfWork.KycDocumentRepository.GetByPlayerIdAsync(playerId)
                ?? throw new EntityNotFoundException(nameof(KycDocument), $"KYC document not found for player {playerId}.");

            var attachment = await _unitOfWork.AttachmentRepository.GetByKycDocumentIdAsync(kycDocument.Id)
                ?? throw new EntityNotFoundException(nameof(Attachment), "No KYC attachment found.");

            var sasUrl = _fileStorageService.GetSasUrl(attachment.BlobName);

            return new AttachmentReadOnlyDTO
            {
                FileName = attachment.FileName,
                DownloadUrl = sasUrl,
                ContentType = attachment.ContentType,
                Extension = attachment.Extension
            };
        }

        /// <summary>
        /// Retrieves a game's full details by its ID.
        /// </summary>
        /// <param name="id">The ID of the game to retrieve.</param>
        /// <returns>
        /// A <see cref="GameReadOnlyDTO"/> containing the complete game information.
        /// </returns>
        public async Task<GameReadOnlyDTO> GetGameByIdAsync(Guid id)
        {
            var game = await _unitOfWork.GameRepository.GetAsync(id)
                ?? throw new EntityNotFoundException(nameof(Game), "Game with id: " + id + " not found");
            return _mapper.Map<GameReadOnlyDTO>(game);
        }

        /// <summary>
        /// Retrieves a paginated list of games filtered by search criteria.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of records returned per page.</param>
        /// <param name="search">An optional search term to filter games by name or other searchable fields.</param>
        /// <returns>
        /// A <see cref="PaginatedResult{GameReadOnlyDTO}"/> containing the filtered and paginated list of games.
        /// </returns>
        public async Task<PaginatedResult<GameReadOnlyDTO>> GetPaginatedGamesFilteredAsync(int pageNumber, int pageSize, string? search)
        {
            var paginatedGames = await _unitOfWork.GameRepository
               .GetPaginatedGamesFilteredAsync(pageNumber, pageSize, search);

            var gameDtos = _mapper.Map<List<GameReadOnlyDTO>>(paginatedGames.Data);

            return new PaginatedResult<GameReadOnlyDTO>
            {
                Data = gameDtos,
                TotalRecords = paginatedGames.TotalRecords,
                PageNumber = paginatedGames.PageNumber,
                PageSize = paginatedGames.PageSize
            };
        }

        /// <summary>
        /// Updates the enabled status of a game.
        /// </summary>
        /// <param name="id">The ID of the game whose status is being updated.</param>
        /// <param name="isEnabled">The new enabled status to apply to the game.</param>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when a game with the specified ID cannot be found.
        /// </exception>
        public async Task ChangeGameStatusAsync(Guid id, bool isEnabled)
        {
            var game = await _unitOfWork.GameRepository.GetAsync(id)
                ?? throw new EntityNotFoundException(nameof(Game), "Game with id: " + id + " not found");

            game.IsEnabled = isEnabled;
            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Game {Id} status changed to {Status}.", id, isEnabled);
        }
    }
}