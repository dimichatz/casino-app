import {Accordion, AccordionContent, AccordionItem, AccordionTrigger} from "@/components/ui/accordion";
import { NavLink, useLocation } from "react-router-dom";
import {CreditCard, LayoutDashboard, UserPen} from "lucide-react";
import { useTranslation } from "react-i18next";
import {useEffect, useState} from "react";

const getAccordionFromPath = (pathname: string): string => {
    if (
        pathname.startsWith("/account/deposit") ||
        pathname.startsWith("/account/withdraw") ||
        pathname.startsWith("/account/history")
    ) {
        return "transactions";
    }

    if (pathname.startsWith("/account/profile")) {
        return "profile";
    }

    return "";
};

const AccountSideMenu = () => {
    const { t } = useTranslation("account");
    const location = useLocation();

    const [openAccordion, setOpenAccordion] = useState<string>(
        getAccordionFromPath(location.pathname)
    );

    useEffect(() => {
        setOpenAccordion(getAccordionFromPath(location.pathname));
    }, [location.pathname]);

    return (
        <div className="rounded-none bg-transparent border-none p-3">
            <div className="mt-4 flex flex-col gap-2">

                {/* OVERVIEW */}
                <NavLink
                    to="/account/overview"
                    className={({ isActive }) => `
                        rounded-md rounded-es-3xl font-medium w-45 h-10 px-4 flex items-center text-md
                    ${isActive
                        ? "bg-white  text-casinoapp-black pointer-events-none"
                        : "text-white hover:bg-white/30"}
                    `}
                >
                    <LayoutDashboard className="h-4 w-4 mr-2" />
                    {t("sidebar.overview")}
                </NavLink>

                <Accordion
                    type="single"
                    collapsible
                    value={openAccordion}
                    onValueChange={setOpenAccordion}
                    className="flex flex-col gap-1"
                >
                    {/* TRANSACTIONS */}
                    <AccordionItem value="transactions" className="border-none">
                        <AccordionTrigger
                            className="
                                rounded-md rounded-es-3xl w-45 h-10 px-4 justify-start text-md
                                text-white hover:bg-white/30 hover:no-underline
                                data-[state=open]:bg-white
                                data-[state=open]:text-casinoapp-black
                            "
                        >
                            <div className="flex items-center w-full">
                                <CreditCard className="h-4 w-4 mr-2" />
                                {t("sidebar.transactions.title")}
                            </div>
                        </AccordionTrigger>

                        <AccordionContent className="pb-1">
                            {[
                                { to: "/account/deposit", label: t("sidebar.transactions.deposit") },
                                { to: "/account/withdraw", label: t("sidebar.transactions.withdraw") },
                                { to: "/account/history", label: t("sidebar.transactions.history") },
                            ].map(({ to, label }) => (
                                <NavLink
                                    key={to}
                                    to={to}
                                    className={({ isActive }) => `
                                        bg-casinoapp-dark-green ml-4 mb-1 pl-4 text-sm
                                        rounded-md flex items-center h-9 transition-colors
                                    ${isActive
                                        ? "font-bold text-[0.9375rem] cursor-default pointer-events-none"
                                        : "cursor-pointer text-zinc-200 hover:bg-casinoapp-dark-green hover:text-white"}
                                    `}
                                >
                                    {label}
                                </NavLink>
                            ))}
                        </AccordionContent>
                    </AccordionItem>

                    {/* PROFILE */}
                    <AccordionItem value="profile" className="border-none">
                        <AccordionTrigger
                            className="
                                rounded-md rounded-es-3xl w-45 h-10 px-4 justify-start text-md
                                text-white hover:bg-white/30 hover:no-underline
                                data-[state=open]:bg-white
                                data-[state=open]:text-casinoapp-black
                            "
                        >
                            <div className="flex items-center w-full">
                                <UserPen className="h-4 w-4 mr-2" />
                                {t("profile:sidebar.title")}
                            </div>
                        </AccordionTrigger>

                        <AccordionContent className="pb-1">
                            {[
                                { to: "/account/profile", label: t("profile:sidebar.edit"), end: true },
                                { to: "/account/profile/security", label: t("profile:sidebar.security") },
                                { to: "/account/profile/protection", label: t("profile:sidebar.protection") },
                                { to: "/account/profile/verification", label: t("profile:sidebar.verification") },
                            ].map(({ to, label, end }) => (
                                <NavLink
                                    key={to}
                                    to={to}
                                    end={end}
                                    className={({ isActive }) => `
                                        bg-casinoapp-dark-green ml-4 mb-1 pl-2 text-sm
                                        rounded-md flex items-center h-9 transition-colors
                                    ${isActive
                                        ? "font-bold text-[0.9375rem] cursor-default pointer-events-none"
                                        : "cursor-pointer text-zinc-200 hover:bg-casinoapp-dark-green hover:text-white"}
                                    `}
                                >
                                    {label}
                                </NavLink>
                            ))}
                        </AccordionContent>
                    </AccordionItem>
                </Accordion>
            </div>
        </div>
    );
};

export default AccountSideMenu;
