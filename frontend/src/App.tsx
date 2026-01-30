import Layout from "@/components/layouts/player/Layout.tsx";
import {BrowserRouter, Routes, Route, Navigate} from "react-router";
import GamesPage from "@/pages/GamesPage.tsx";
import {UIProvider} from "@/context/UIProvider.tsx";
import {toast, Toaster} from "sonner";
import ProtectedRoute from "@/routes/ProtectedRoute.tsx";
import ScrollToTop from "@/components/ScrollToTop.tsx";
import AccountLayout from "@/pages/account/AccountLayout.tsx";
import Overview from "@/pages/account/Overview.tsx";
import Deposit from "@/pages/account/transactions/Deposit.tsx";
import Withdraw from "@/pages/account/transactions/Withdraw.tsx";
import TransactionHistory from "@/pages/account/transactions/TransactionHistory.tsx";
import {TransactionFiltersProvider} from "@/context/TransactionFiltersContext.tsx";
import IdentityVerification from "@/pages/account/profile/IdentityVerification.tsx";
import PlayerProtection from "@/pages/account/profile/PlayerProtection.tsx";
import Security from "@/pages/account/profile/Security.tsx";
import PersonalDetails from "@/pages/account/profile/PersonalDetails.tsx";
import NotFoundPage from "@/pages/NotFoundPage.tsx";
import {useTranslation} from "react-i18next";
import {useEffect} from "react";

function App() {
    const { t } = useTranslation("games");

    const GAME_ORIGIN = "https://localhost:5001";

    useEffect(() => {
        const handler = (event: MessageEvent) => {
            if (event.origin !== GAME_ORIGIN) return;

            if (event.data?.source !== "HIGHLOW_GAME") return;

            if (event.data.type === "LOSS_LIMIT_REACHED") {
                toast.warning(
                    t(`errors.lossLimit.${event.data.period}`)
                );
            }
        };

        window.addEventListener("message", handler);
        return () => window.removeEventListener("message", handler);
    }, [t]);

    return (
        <>
            <BrowserRouter>
                <TransactionFiltersProvider>
                    <UIProvider>
                        <ScrollToTop />
                        <Toaster richColors position="bottom-right" />

                        <Routes>
                            <Route element={<Layout />}>
                                <Route index element={<Navigate to="/casino" replace />}/>
                                <Route path="casino" element={<GamesPage />}/>

                                <Route element={<ProtectedRoute />}>
                                    <Route path="account" element={<AccountLayout />}>
                                        <Route index element={<Overview />}/>
                                        <Route path="overview" element={<Overview />}/>
                                        <Route path="deposit" element={<Deposit />}/>
                                        <Route path="withdraw" element={<Withdraw />}/>
                                        <Route path="history" element={<TransactionHistory />}/>
                                        <Route path="profile" element={<PersonalDetails />}/>
                                        <Route path="profile/security" element={<Security />}/>
                                        <Route path="profile/protection" element={<PlayerProtection />}/>
                                        <Route path="profile/verification" element={<IdentityVerification />}/>
                                    </Route>
                                </Route>

                                <Route path="*" element={<NotFoundPage />} />
                            </Route>
                        </Routes>
                    </UIProvider>
                </TransactionFiltersProvider>
            </BrowserRouter>
        </>
    )
}

export default App