import { Outlet } from "react-router-dom";
import { useEffect } from "react";
import { useAuth } from "@/hooks/useAuth";
import { useUI } from "@/hooks/useUI";

const ProtectedRoute = () => {
    const { isAuthenticated, logoutReason } = useAuth();
    const { openLogin, activeModal } = useUI();

    useEffect(() => {
        if (
            !isAuthenticated &&
            logoutReason !== "logout" &&
            activeModal !== "login"
        ) {
            openLogin();
        }
    }, [isAuthenticated, logoutReason, activeModal, openLogin]);

    if (!isAuthenticated) {
        return null;
    }

    return <Outlet />;

};

export default ProtectedRoute;
