import AccountSideMenu from "@/components/account/AccountSideMenu.tsx";
import {Outlet, useLocation} from "react-router";
import {useEffect, useState} from "react";
import {useTranslation} from "react-i18next";

const AccountLayout = () => {
    const { t } = useTranslation("account");
    const [mobileMenuOpen, setMobileMenuOpen] = useState(false);
    const location = useLocation();

    useEffect(() => {
        setMobileMenuOpen(false);
    }, [location.pathname]);

    return (
        <div className="flex">

            <button
                onClick={() => setMobileMenuOpen(true)}
                style={{
                    writingMode: "vertical-rl",
                }}
                className="
                        fixed md:hidden left-0 top-1/2 -translate-y-1/2
                        bg-casinoapp-dark-blue
                        px-4 py-2 rounded-r-xl shadow-xl
                        text-md font-semibold text-white
                        border-3 border-l-0 border-casinoapp-light-lime-green"
            >
                {t("sidebar.title")}
            </button>
            {mobileMenuOpen && (
                <div
                    onClick={() => setMobileMenuOpen(false)}
                    className="fixed top-16 inset-x-0
                        bottom-0 bg-black/70 z-40 md:hidden"
                />
            )}

            <aside
                className={`
                        fixed md:sticky 
                        z-50 md:z-auto 
                        w-52 
                        h-screen top-16
                        bg-casinoapp-dark-green text-white
                        transform transition-transform duration-300 ease-in-out
                        ${mobileMenuOpen ? "translate-x-0" : "-translate-x-full"}
                        md:translate-x-0
          `}>
                <AccountSideMenu/>
            </aside>

            <main className="flex flex-1 justify-center pb-4">
                <div className="flex justify-center w-full max-w-6xl">
                    <Outlet />
                </div>
            </main>
        </div>
    );
};

export default AccountLayout;