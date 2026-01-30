import { api } from "@/services/api";
import type {PaginatedResult, TransactionReadOnlyDTO} from "@/types/transactions-api";
import type {TransactionFilters} from "@/types/transaction-filters.ts";
import type {TransactionFiltersState} from "@/context/TransactionFiltersContext.tsx";

export const transactionService = {
    deposit: async (
        payload: { amount: number }
    ) => {
        const res = await api.post<TransactionReadOnlyDTO>(
            "/player/transactions/deposit",
            payload
        );
        return res.data;
    },

    withdraw: async (
        payload: { amount: number }
    ) => {
        const res = await api.post<TransactionReadOnlyDTO>(
            "/player/transactions/withdraw",
            payload
        );
        return res.data;
    },

    getTransactions: async (params: {
        pageNumber: number;
        pageSize: number;
    } & TransactionFilters) => {

        const res =
            await api.get<PaginatedResult<TransactionReadOnlyDTO>>(
            "/player/transactions",
            { params }
        );

        return res.data;
    },

    downloadTransactions: async (
        filters: TransactionFiltersState
    ) => {
        const res =
            await api.get<Blob>("/player/transactions/download", {
            params: filters,
            responseType: "blob",
        });

        return res.data;
    },
};
