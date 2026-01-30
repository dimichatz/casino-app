import type {TransactionStatus, TransactionType} from "@/schemas/core/enums.ts";

export interface TransactionRequestDTO {
    transactionType?: TransactionType;
    amount: number;
    currency?: string;
    gameId?: string;
    gameRoundId?: number;
    betAmount?: number;
}

export interface TransactionReadOnlyDTO {
    id: string;
    transactionType: TransactionType;
    transactionStatus: TransactionStatus;
    transactionNumber: number;
    gameId?: string | null;
    gameRoundId?: string | null;
    gameName?: string | null;
    amount: number;
    currency: string;
    oldBalance?: number | null;
    newBalance?: number | null;
    insertedAt: string;
}

export interface PaginatedResult<T> {
    data: T[];
    totalRecords: number;
    pageNumber: number;
    pageSize: number;
    totalPages: number;
}