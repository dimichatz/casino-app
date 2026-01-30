import { createContext } from "react";

export type UIContextType = {
    activeModal:
        "login"
        | "register"
        | "transaction-search"
        | "transaction-filters"
        | null;
    openLogin: () => void;
    openRegister: () => void;
    openTransactionSearch: () => void;
    openTransactionFilters: () => void;
    closeModal: () => void;
};

export const UIContext = createContext<UIContextType | null>(null);
