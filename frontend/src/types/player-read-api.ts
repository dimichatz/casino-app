import type {Gender, KycStatus, SelfExclusionPeriod, UserRole} from "@/schemas/core/enums.ts";

export interface PlayerFullDetails {
    id: string;
    userDetails?: PlayerUserDetails;
    addressDetails?: PlayerAddressDetails;
    kycDetails?: PlayerKycDetails;
    selfExclusionAndLimitDetails?: PlayerSelfExclusionAndLimitDetails;
}

export interface PlayerUserDetails {
    userId: string;
    username?: string;
    email?: string;
    phoneNumber?: string;
    userRole?: UserRole;
    isActive?: boolean;
}

export interface PlayerAddressDetails {
    streetName?: string;
    streetNumber?: string;
    postalCode?: string;
    city?: string;
    countryCode?: string;
}

export interface PlayerKycDetails {
    isKycVerified?: boolean;
    firstname?: string;
    lastname?: string;
    birthDate?: string;
    gender?: Gender;
    documentType?: DocumentType;
    documentNumber?: string;
    expireDate?: string;
    kycStatus?: KycStatus;
    kycCheckDate?: string;
    kycCheckedBy?: string;
}

export interface PlayerSelfExclusionAndLimitDetails {
    isSelfExcluded?: boolean;
    selfExclusionStart?: string;
    selfExclusionEnd?: string | null;
    selfExclusionPeriod?: SelfExclusionPeriod;

    depositDailyLimit?: number;
    pendingDepositDailyLimit?: number | null;
    pendingDepositDailyLimitStart?: string | null;
    depositWeeklyLimit?: number;
    pendingDepositWeeklyLimit?: number | null;
    pendingDepositWeeklyLimitStart?: string | null;
    depositMonthlyLimit?: number;
    pendingDepositMonthlyLimit?: number | null;
    pendingDepositMonthlyLimitStart?: string | null;

    lossDailyLimit?: number;
    pendingLossDailyLimit?: number | null;
    pendingLossDailyLimitStart?: string | null;
    lossWeeklyLimit?: number;
    pendingLossWeeklyLimit?: number | null;
    pendingLossWeeklyLimitStart?: string | null;
    lossMonthlyLimit?: number;
    pendingLossMonthlyLimit?: number | null;
    pendingLossMonthlyLimitStart?: string | null;
}