import { useState } from "react";
import { UIContext } from "@/context/UIContext.ts";
import ModalRoot from "@/components/auth/ModalRout";

type Props = {
    children: React.ReactNode;
};

export type ModalType = "login" | "register"
    | "transaction-search" | "transaction-filters" | null;

export const UIProvider = ({ children }: Props) => {
    const [activeModal, setActiveModal] = useState<ModalType>(null);

    const openLogin = () => setActiveModal("login");
    const openRegister = () => setActiveModal("register");
    const openTransactionSearch = () => setActiveModal("transaction-search");
    const openTransactionFilters = () => setActiveModal("transaction-filters");
    const closeModal  = () => setActiveModal(null);


    return (
        <UIContext.Provider
            value={{
                activeModal,
                openLogin,
                openRegister,
                openTransactionSearch,
                openTransactionFilters,
                closeModal,
            }}
        >
            {children}
            <ModalRoot />
        </UIContext.Provider>
    );
};
