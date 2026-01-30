export const GENDER_VALUES = ["Male", "Female"] as const;
export type Gender = typeof GENDER_VALUES[number];

export const USER_ROLE_VALUES = [
    "Player",
    "Admin",
    "SuperAdmin",
] as const;
export type UserRole = typeof USER_ROLE_VALUES[number];

export const KYC_STATUS_VALUES = [
    "Pending",
    "Approved",
    "Rejected",
] as const;
export type KycStatus = typeof KYC_STATUS_VALUES[number];

export const DOCUMENT_TYPE_VALUES = [
    "IdCard",
    "Passport"
] as const;
export type DocumentType = typeof DOCUMENT_TYPE_VALUES[number];

export const TRANSACTION_TYPE_VALUES = [
    "Deposit",
    "Withdraw",
    "Bet",
    "Win",
    "Tax",
    "Bonus",
] as const;
export type TransactionType = typeof TRANSACTION_TYPE_VALUES[number];

export const TRANSACTION_STATUS_VALUES = [
    "Pending",
    "Completed",
    "Failed",
    "Cancelled",
] as const;
export type TransactionStatus = typeof TRANSACTION_STATUS_VALUES[number];

export const SelfExclusionPeriod = {
    OneDay: 1,
    OneWeek: 7,
    OneMonth: 30,
    TwoMonths: 60,
    SixMonths: 180,
    OneYear: 365,
    Permanent: -1,
} as const;

export type SelfExclusionPeriod =
    typeof SelfExclusionPeriod[keyof typeof SelfExclusionPeriod];

export const SELF_EXCLUSION_PERIOD_VALUES: SelfExclusionPeriod[] = [
    SelfExclusionPeriod.OneDay,
    SelfExclusionPeriod.OneWeek,
    SelfExclusionPeriod.OneMonth,
    SelfExclusionPeriod.TwoMonths,
    SelfExclusionPeriod.SixMonths,
    SelfExclusionPeriod.OneYear,
    SelfExclusionPeriod.Permanent,
];