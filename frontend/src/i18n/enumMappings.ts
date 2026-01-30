import type {TransactionType, TransactionStatus} from "@/schemas/core/enums";
import { SelfExclusionPeriod } from "@/schemas/core/enums";

export const TRANSACTION_TYPE_I18N_KEY: Record<TransactionType, string> = {
    Deposit: "transactions.types.deposit",
    Withdraw: "transactions.types.withdraw",
    Bet: "transactions.types.bet",
    Win: "transactions.types.win",
    Tax: "transactions.types.tax",
    Bonus: "transactions.types.bonus",
};

export const TRANSACTION_STATUS_I18N_KEY: Record<TransactionStatus, string> = {
    Pending: "transactions.status.pending",
    Completed: "transactions.status.completed",
    Failed: "transactions.status.failed",
    Cancelled: "transactions.status.cancelled",
};

export const SELF_EXCLUSION_PERIOD_I18N_KEY: Record<
    SelfExclusionPeriod,
    string
> = {
    [SelfExclusionPeriod.OneDay]: "protection.exclusion.oneDay",
    [SelfExclusionPeriod.OneWeek]: "protection.exclusion.oneWeek",
    [SelfExclusionPeriod.OneMonth]: "protection.exclusion.oneMonth",
    [SelfExclusionPeriod.TwoMonths]: "protection.exclusion.twoMonths",
    [SelfExclusionPeriod.SixMonths]: "protection.exclusion.sixMonths",
    [SelfExclusionPeriod.OneYear]: "protection.exclusion.oneYear",
    [SelfExclusionPeriod.Permanent]: "protection.exclusion.permanent",
};


