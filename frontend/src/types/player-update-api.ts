import type { SelfExclusionPeriod } from "@/schemas/core/enums";

export interface PlayerUpdateRequest {
    userDetails?: PlayerUpdateUserDetails
    addressDetails?: PlayerUpdateAddressDetails;
    selfExclusionDetails?: PlayerUpdateSelfExclusionDetails;
    limitDetails?: PlayerUpdateLimitDetails;
}

export interface PlayerUpdateUserDetails {
    email?: string;
    phoneNumber?: string;
}

export interface PlayerUpdateAddressDetails {
    streetName?: string;
    streetNumber?: string;
    postalCode?: string;
    city?: string;
}

export interface PlayerUpdateSelfExclusionDetails {
    selfExclusionPeriod?: SelfExclusionPeriod;
}

export interface PlayerUpdateLimitDetails {
    depositDailyLimit?: number;
    depositWeeklyLimit?: number;
    depositMonthlyLimit?: number;
    lossDailyLimit?: number;
    lossWeeklyLimit?: number;
    lossMonthlyLimit?: number;
}
