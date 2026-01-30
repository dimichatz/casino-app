import {useTranslation} from "react-i18next";
import BettingLimitsCard from "@/components/account/profile/player-protection/BettingLimitsCard.tsx";
import SelfExclusionCard from "@/components/account/profile/player-protection/SelfExclusionCard.tsx";
import {useDocumentTitle} from "@/hooks/useDocumentTitle.ts";

const PlayerProtection = () => {
    const { t } = useTranslation("profile");
    useDocumentTitle(t("common:documentTitle.protection"));

    return (
        <>
            <div className="flex flex-col text-casinoapp-black w-full max-w-3xl">
                <span className="text-2xl font-bold text-white p-6">
                    {t("protection.title")}
                </span>
                <BettingLimitsCard />
                <SelfExclusionCard />
            </div>
        </>
    );
};

export default PlayerProtection;
