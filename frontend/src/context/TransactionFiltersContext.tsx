import { createContext, useContext, useState } from "react";

export type TransactionFiltersState = {
    transactionNumber: string | null;
    dateStart: string | null;
    dateEnd: string | null;
    includeDeposits: boolean;
    includeWithdrawals: boolean;
    includeCasino: boolean;
    includeOther: boolean;
};

export const defaultFilters: TransactionFiltersState = {
    transactionNumber: null,
    dateStart: null,
    dateEnd: null,
    includeDeposits: false,
    includeWithdrawals: false,
    includeCasino: false,
    includeOther: false,
};

interface TransactionFiltersContextValue {
    filters: TransactionFiltersState;
    setFilters: React.Dispatch<React.SetStateAction<TransactionFiltersState>>;
    clearFilters: () => void;
}

const TransactionFiltersContext =
    createContext<TransactionFiltersContextValue | null>(null);

export const TransactionFiltersProvider = ({
                                               children,
                                           }: {
        children: React.ReactNode;
    }) => {
        const [filters, setFilters] = useState<TransactionFiltersState>(defaultFilters);

        const clearFilters = () => {
            setFilters(defaultFilters);
    };

    return (
        <TransactionFiltersContext.Provider
            value={{ filters, setFilters, clearFilters }}
        >
            {children}
        </TransactionFiltersContext.Provider>
    );
};

export const useTransactionFilters = () => {
    const ctx = useContext(TransactionFiltersContext);
    if (!ctx) {
        throw new Error(
            "useTransactionFilters must be used within TransactionFiltersProvider"
        );
    }
    return ctx;
};
