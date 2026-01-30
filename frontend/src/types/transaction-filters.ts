export interface TransactionFilters {
    transactionNumber?: string;
    dateStart?: string;
    dateEnd?: string;
    includeDeposits?: boolean;
    includeWithdrawals?: boolean;
    includeCasino?: boolean;
    includeOther?: boolean;
}