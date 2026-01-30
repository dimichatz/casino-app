import { useTranslation } from "react-i18next";
import logoDesktop from "@/assets/images/casinoapp_logo_desktop.png";
import logoMobile from "@/assets/images/casinoapp_logo_mobile.png";
import { NavLink } from "react-router-dom";
import {Settings, Search, LogOut, CircleUser, Wallet, Euro, Menu, X} from 'lucide-react';
import { useAuth } from "@/hooks/useAuth.ts";
import { usePlayer } from "@/hooks/usePlayer.ts";
import { useUI } from "@/hooks/useUI.ts";
import {useEffect, useState} from "react";
import { useNavigate } from "react-router-dom";

const Header = () => {
    const { t, i18n } = useTranslation("common");
    const { isAuthenticated, role, username, logoutUser,} = useAuth();
    const { balance } = usePlayer();
    const { openLogin, openRegister } = useUI();
    const [isMenuOpen, setIsMenuOpen] = useState(false);
    const [isSettingsOpen, setIsSettingsOpen] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        const handler = () => {
            if (window.innerWidth >= 1024) {
                setIsMenuOpen(false);
            }
        };

        window.addEventListener("resize", handler);
        return () => window.removeEventListener("resize", handler);
    }, []);

    useEffect(() => {
        const closeAll = () => {
            setIsMenuOpen(false);
            setIsSettingsOpen(false);
        };

        window.addEventListener("click", closeAll);
        return () => window.removeEventListener("click", closeAll);
    }, []);

    const handleLogout = () => {
        navigate("/");
        logoutUser();
    };

    return (
        <>
            <div className="relative h-16 w-full bg-gradient-to-r
                from-casinoapp-orange via-casinoapp-dark-green to-casinoapp-black
                text-casinoapp-light-gray">

                <div className="h-16 px-6 flex items-center">

                    <div className="flex items-center gap-4">
                        <img
                            src={logoMobile}
                            alt="CasinoApp Logo"
                            className="h-8 block md:hidden cursor-pointer"
                            onClick={() => navigate("/")}
                        />

                        <img
                            className="h-8 w-28 mr-4 cursor-pointer hidden md:block"
                            src={logoDesktop}
                            alt="CasinoApp Logo"
                            onClick={() => navigate("/")}
                        />

                        <div className="relative lg:hidden">
                            <button
                                onClick={(e) => {
                                    e.stopPropagation();
                                    setIsMenuOpen((v) => !v);
                                }}
                                className={`p-2 rounded-md transition
                                ${isMenuOpen ? "text-white" : "text-casinoapp-light-gray"}
                                     hover:bg-white/10`}
                            >
                                <Menu />
                            </button>

                            {isMenuOpen && (
                                <div className="absolute m-2 border border-white/10
                                    bg-casinoapp-dark-blue rounded-md shadow-lg"
                                >
                                    <ul className="flex flex-col p-4 gap-4 text-sm">
                                        <li>
                                            <NavLink
                                                to="/"
                                                onClick={() => setIsMenuOpen(false)}
                                                className="hover:text-white"
                                            >
                                                {t("header.menu.casino")}
                                            </NavLink>
                                        </li>
                                    </ul>
                                </div>
                            )}
                        </div>


                        <nav className="hidden lg:block">
                            <ul className="flex items-center gap-6 text-[1.1rem] font-medium">
                                <li>
                                    <NavLink to="/casino" className={({ isActive }) =>
                                        `relative pb-4 transition-all duration-200 rounded
                                            ${isActive ? "text-white border-b-5 border-white" 
                                            : "hover:text-white"}
                                        `
                                    }>
                                        {t("header.casino")}
                                    </NavLink>
                                </li>
                            </ul>
                        </nav>
                    </div>

                    <div className="ml-auto flex items-center gap-4">
                        <Search
                            className="text-casinoapp-light-gray cursor-pointer hover:text-white"
                            onClick={() => {
                                navigate("/casino", {
                                    state: { focusSearch: true },
                                });
                            }}
                        />

                        {/*CHANGE LANGUAGE*/}
                        <div className="relative">
                            <Settings
                                className={`
                                    cursor-pointer transition hover:text-white
                                ${isSettingsOpen ? "text-white" : "text-casinoapp-light-gray"}
                                `}
                                onClick={(e) => {
                                    e.stopPropagation();
                                    setIsSettingsOpen((v) => !v);
                                }}
                            />

                            {isSettingsOpen && (
                                <div
                                    className="absolute right-0 top-10 w-56
                                        bg-casinoapp-dark-blue rounded-lg shadow-lg
                                        border border-white/10 z-50"
                                    onClick={(e) => e.stopPropagation()}
                                >
                                    <div className="flex items-center justify-between px-4 py-3 border-b border-white/10">
                                        <span className="text-sm tracking-wide font-semibold text-white">
                                            {t("header.settings")}
                                        </span>

                                        <button
                                            onClick={() => setIsSettingsOpen(false)}
                                            className="text-white/60 hover:text-white"
                                        >
                                            <X className="h-4 w-4" />
                                        </button>
                                    </div>

                                    <div className="m-2 bg-casinoapp-dark-blue rounded-md">
                                        <button
                                            className={`w-full px-4 py-2 text-left text-sm hover:bg-white/20
                                            rounded-md cursor-pointer
                                            ${i18n.language === "en" ? "text-white" : "text-casinoapp-light-gray"}`}
                                            onClick={() => {
                                                void i18n.changeLanguage("en");
                                                localStorage.setItem("lang", "en");
                                                setIsSettingsOpen(false);
                                            }}
                                        >
                                            <div className="flex items-center gap-2">
                                                <span className="fi fi-gb fis rounded-full text-xl" />
                                                <span className="font-normal text-sm">English</span>
                                            </div>

                                        </button>

                                        <button
                                            className={`w-full px-4 py-2 text-left text-sm hover:bg-white/20
                                            rounded-md cursor-pointer
                                            ${i18n.language === "el" ? "text-white" : ""}`}
                                            onClick={() => {
                                                void i18n.changeLanguage("el");
                                                localStorage.setItem("lang", "el");
                                                setIsSettingsOpen(false);
                                            }}
                                        >
                                            <div className="flex items-center gap-2">
                                                <span className="fi fi-gr fis rounded-full text-xl" />
                                                <span className="font-normal text-sm">Ελληνικά</span>
                                            </div>
                                        </button>
                                    </div>

                                </div>
                            )}
                        </div>

                        {/*LOGGED-IN PLAYER*/}

                        {!isAuthenticated && (
                            <div className="flex items-center gap-4">
                                <button
                                    onClick={openRegister}
                                    className="px-2.5 py-1.5 text-[0.8rem] font-medium rounded-md
                                        text-white border-2 border-casinoapp-orange
                                        hover:bg-casinoapp-dark-green/50 transition cursor-pointer"
                                >
                                    {t("header.register")}
                                </button>

                                <button
                                    onClick={openLogin}
                                    className="relative overflow-hidden px-3 py-2 text-[0.8rem] font-bold
                                        rounded-md text-casinoapp-black cursor-pointer
                                        bg-casinoapp-lime-green hover:bg-casinoapp-light-lime-green"
                                >
                                    {t("header.login")}
                                </button>
                            </div>
                        )}

                        {isAuthenticated && role === "Player" && (
                            <div className="flex items-center gap-4">
                                <NavLink to="account/profile"
                                         className="relative px-2.5 py-1.5 text-sm font-semibold
                                            flex items-center text-white rounded-l-lg
                                            ring-2 ring-casinoapp-orange transition
                                            hover:shadow-[0_4px_12px_rgba(189,125,74,0.6)]"
                                >
                                    <CircleUser className="mr-0.5" />
                                    <span className="hidden md:inline">
                                    {username}
                                </span>
                                </NavLink>

                                <NavLink to="/account/overview"
                                         className="relative px-2.5 py-1.5 text-sm font-semibold
                                            flex items-center text-white rounded-r-lg
                                            ring-2 ring-casinoapp-orange transition
                                            hover:shadow-[0_4px_12px_rgba(189,125,74,0.6)]"
                                >
                                    {balance === null ? (
                                        <div className="flex items-center animate-pulse">
                                            <div className="h-4 w-14 rounded bg-muted-foreground/20" />
                                        </div>
                                    ) : (
                                        <>
                                            {balance.toLocaleString("el-GR", {
                                                minimumFractionDigits: 2,
                                                maximumFractionDigits: 2,
                                            })}
                                            <Euro className="w-4 h-4" />
                                        </>
                                    )}

                                    <Wallet className="w-6 h-6 ml-1" />
                                </NavLink>

                                <LogOut
                                    onClick={handleLogout}
                                    strokeWidth={2.5}
                                    className=" w-8 h-8 ml-2
                                        text-casinoapp-light-gray hover:text-white"
                                />
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </>
    );
};
export default Header;