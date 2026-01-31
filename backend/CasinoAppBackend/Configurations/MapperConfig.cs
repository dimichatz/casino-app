using AutoMapper;
using CasinoAppBackend.Data;
using CasinoAppBackend.DTO;
using CasinoAppBackend.DTO.AuditReadOnlyDTO;
using CasinoAppBackend.DTO.PlayerFullDetailsReadOnlyDTO;
using CasinoAppBackend.DTO.PlayerUpdateFullDetailsAdminDTO;
using CasinoAppBackend.DTO.PlayerUpdateFullDetailsPlayerDTO;

namespace CasinoAppBackend.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            // Map AdminCreateDTO to User and Admin entities
            CreateMap<AdminCreateDTO, User>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<AdminCreateDTO, Admin>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Map AdminUpdateDTO to User and Admin entities
            CreateMap<AdminUpdateDTO, User>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
            srcMember != null && !(srcMember is string str && string.IsNullOrWhiteSpace(str))));
            CreateMap<AdminUpdateDTO, Admin>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
            srcMember != null && !(srcMember is string str && string.IsNullOrWhiteSpace(str))));

            // Map Admin to AdminReadOnlyDTO
            CreateMap<Admin, AdminReadOnlyDTO>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.User.Firstname))
                .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.User.Lastname))
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.User.UserRole))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.User.IsActive));

            // Map Player to PlayerFullDetailsDTO
            CreateMap<Player, PlayerFullDetailsReadOnlyDTO>()
                .ForMember(dest => dest.UserDetails, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.AddressDetails, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.KycDetails, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.SelfExclusionAndLimitDetails, opt => opt.MapFrom(src => src));

            CreateMap<User, PlayerUserDetailsReadOnlyDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));
            CreateMap<Player, PlayerAddressDetailsReadOnlyDTO>();
            CreateMap<Player, PlayerSelfExclusionAndLimitDetailsReadOnlyDTO>()
                .ForMember(dest => dest.IsSelfExcluded, opt => opt.MapFrom(src => src.IsSelfExcluded))
                .ForMember(dest => dest.SelfExclusionStart, opt => opt.MapFrom(src => src.SelfExclusionStart))
                .ForMember(dest => dest.SelfExclusionEnd, opt => opt.MapFrom(src => src.SelfExclusionEnd))
                .ForMember(dest => dest.SelfExclusionPeriod, opt => opt.MapFrom(src => src.SelfExclusionPeriod))
                .ForMember(dest => dest.DepositDailyLimit, opt => opt.MapFrom(src => src.PlayerLimit.DepositDailyLimit))
                .ForMember(dest => dest.DepositWeeklyLimit, opt => opt.MapFrom(src => src.PlayerLimit.DepositWeeklyLimit))
                .ForMember(dest => dest.DepositMonthlyLimit, opt => opt.MapFrom(src => src.PlayerLimit.DepositMonthlyLimit))
                .ForMember(dest => dest.PendingDepositDailyLimit, opt => opt.MapFrom(src => src.PlayerLimit.PendingDepositDailyLimit))
                .ForMember(dest => dest.PendingDepositDailyLimitStart, opt => opt.MapFrom(src => src.PlayerLimit.PendingDepositDailyLimitStart))
                .ForMember(dest => dest.PendingDepositWeeklyLimit, opt => opt.MapFrom(src => src.PlayerLimit.PendingDepositWeeklyLimit))
                .ForMember(dest => dest.PendingDepositWeeklyLimitStart, opt => opt.MapFrom(src => src.PlayerLimit.PendingDepositWeeklyLimitStart))
                .ForMember(dest => dest.PendingDepositMonthlyLimit, opt => opt.MapFrom(src => src.PlayerLimit.PendingDepositMonthlyLimit))
                .ForMember(dest => dest.PendingDepositMonthlyLimitStart, opt => opt.MapFrom(src => src.PlayerLimit.PendingDepositMonthlyLimitStart))
                .ForMember(dest => dest.LossDailyLimit, opt => opt.MapFrom(src => src.PlayerLimit.LossDailyLimit))
                .ForMember(dest => dest.LossWeeklyLimit, opt => opt.MapFrom(src => src.PlayerLimit.LossWeeklyLimit))
                .ForMember(dest => dest.LossMonthlyLimit, opt => opt.MapFrom(src => src.PlayerLimit.LossMonthlyLimit))
                .ForMember(dest => dest.PendingLossDailyLimit, opt => opt.MapFrom(src => src.PlayerLimit.PendingLossDailyLimit))
                .ForMember(dest => dest.PendingLossDailyLimitStart, opt => opt.MapFrom(src => src.PlayerLimit.PendingLossDailyLimitStart))
                .ForMember(dest => dest.PendingLossWeeklyLimit, opt => opt.MapFrom(src => src.PlayerLimit.PendingLossWeeklyLimit))
                .ForMember(dest => dest.PendingLossWeeklyLimitStart, opt => opt.MapFrom(src => src.PlayerLimit.PendingLossWeeklyLimitStart))
                .ForMember(dest => dest.PendingLossMonthlyLimit, opt => opt.MapFrom(src => src.PlayerLimit.PendingLossMonthlyLimit))
                .ForMember(dest => dest.PendingLossMonthlyLimitStart, opt => opt.MapFrom(src => src.PlayerLimit.PendingLossMonthlyLimitStart));
            CreateMap<Player, PlayerKycDetailsReadOnlyDTO>()
                .ForMember(dest => dest.IsKycVerified, opt => opt.MapFrom(src => src.IsKycVerified))
                .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.User.Firstname))
                .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.User.Lastname))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.KycDocument.DocumentType))
                .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.KycDocument.DocumentNumber))
                .ForMember(dest => dest.ExpireDate, opt => opt.MapFrom(src => src.KycDocument.ExpireDate))
                .ForMember(dest => dest.KycStatus, opt => opt.MapFrom(src => src.KycDocument.KycStatus))
                .ForMember(dest => dest.KycCheckDate, opt => opt.MapFrom(src => src.KycDocument.KycCheckDate))
                .ForMember(dest => dest.KycCheckedBy, opt => opt.MapFrom(src => src.KycDocument.KycCheckedBy));

            // Map PlayerUpdateFullDetailsAdminDTO sub-DTOs to Player, User and KycDocument entities
            CreateMap<PlayerUpdateUserStatusDetailsAdminDTO, User>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
            srcMember != null && !(srcMember is string str && string.IsNullOrWhiteSpace(str))));
            CreateMap<PlayerUpdateUserDetailsAdminDTO, User>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
            srcMember != null && !(srcMember is string str && string.IsNullOrWhiteSpace(str))));
            CreateMap<PlayerUpdateAddressDetailsAdminDTO, Player>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
            srcMember != null && !(srcMember is string str && string.IsNullOrWhiteSpace(str))));
            CreateMap<PlayerUpdateKycDetailsAdminDTO, Player>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
            srcMember != null && !(srcMember is string str && string.IsNullOrWhiteSpace(str))));
            CreateMap<PlayerUpdateKycDetailsAdminDTO, User>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
            srcMember != null && !(srcMember is string str && string.IsNullOrWhiteSpace(str))));
            CreateMap<PlayerUpdateKycDetailsAdminDTO, KycDocument>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
            srcMember != null && !(srcMember is string str && string.IsNullOrWhiteSpace(str))));

            // Map PlayerSignupDTO to User, Player and KYCDocument entities
            CreateMap<PlayerSignUpDTO, User>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<PlayerSignUpDTO, Player>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<PlayerSignUpDTO, KycDocument>()
                .ForMember(dest => dest.PlayerId, opt => opt.Ignore())
                .ForMember(dest => dest.Attachment, opt => opt.Ignore());

            // Map PlayerUpdateFullDetailsPlayerDTO sub-DTOs to Player, User and PlayerLimit entities
            CreateMap<PlayerUpdateUserDetailsPlayerDTO, User>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
            srcMember != null && !(srcMember is string str && string.IsNullOrWhiteSpace(str))));
            CreateMap<PlayerUpdateAddressDetailsPlayerDTO, Player>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
            srcMember != null && !(srcMember is string str && string.IsNullOrWhiteSpace(str))));
            CreateMap<PlayerUpdateSelfExclusionDetailsPlayerDTO, Player>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
            srcMember != null && !(srcMember is string str && string.IsNullOrWhiteSpace(str))));
            CreateMap<PlayerUpdateLimitDetailsPlayerDTO, PlayerLimit>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
            srcMember != null && !(srcMember is string str && string.IsNullOrWhiteSpace(str))));

            // Map Audits to AuditReadOnlyDTOs
            CreateMap<PlayerSelfExclusionAudit, PlayerSelfExclusionAuditReadOnlyDTO>();
            CreateMap<PlayerLimitAudit, PlayerLimitAuditReadOnlyDTO>();
            CreateMap<PlayerDetailsAudit, PlayerDetailsAuditReadOnlyDTO>();
            CreateMap<PlayerBanAudit, PlayerBanAuditReadOnlyDTO>();

            // Map Transaction to TransactionReadOnlyDTO
            CreateMap<Transaction, TransactionReadOnlyDTO>();

            // Map Player to PlayerReadOnlyDTO
            CreateMap<Player, PlayerReadOnlyDTO>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.User.UserRole))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.User.IsActive));

            // Map Game to GameReadOnlyDTO
            CreateMap<Game, GameReadOnlyDTO>();
        }
    }
}
