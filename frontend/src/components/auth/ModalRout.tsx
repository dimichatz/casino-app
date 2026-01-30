import { useUI } from "@/hooks/useUI";

import LoginModal from "@/components/auth/LoginModal";
import RegisterModal from "@/components/auth/RegisterModal";
import FiltersModal from "@/components/account/transactions/FiltersModal";
import SearchModal from "@/components/account/transactions/SearchModal";

const ModalRoot = () => {
    const { activeModal } = useUI();

    switch (activeModal) {
        case "login":
            return <LoginModal />;

        case "register":
            return <RegisterModal />;

        case "transaction-search":
            return <SearchModal />;

        case "transaction-filters":
            return <FiltersModal />;

        default:
            return null;
    }
};

export default ModalRoot;
