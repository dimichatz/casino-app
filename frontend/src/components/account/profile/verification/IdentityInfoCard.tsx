import CollapsibleCard from "@/components/account/profile/verification/CollapsibleCard.tsx";
import {useTranslation} from "react-i18next";
import type {PlayerFullDetails} from "@/types/player-read-api.ts";
import i18n from "i18next";
import {formatDateLocal} from "@/utils/date.ts";
import {IdCard} from "lucide-react";

interface IdentityInfoCardProps {
    kyc: PlayerFullDetails["kycDetails"] | null;
}

const IdentityInfoCard = ({ kyc }: IdentityInfoCardProps) => {
    const { t } = useTranslation("profile");

    if (!kyc) {
        return null;
    }

    return(
        <CollapsibleCard
            title={
            <div className="flex gap-2">
                <IdCard />
                {t("verification.identity.title")}
            </div>
            }
        >
            <div
                className="flex flex-col w-full max-w-sm gap-4 select-none pointer-events-none p-4
                bg-muted rounded-xl"
            >
                <div>
                    <label className="block text-xs font-bold text-casinoapp-black mb-1">
                        {t("auth:register.firstname")}
                    </label>
                    <div className="h-6 flex items-center text-sm text-muted-foreground">
                        {kyc.firstname}
                    </div>
                </div>
                <div>
                    <label className="block text-xs font-bold text-casinoapp-black mb-1">
                        {t("auth:register.lastname")}
                    </label>
                    <div className="h-6 flex items-center text-sm text-muted-foreground">
                        {kyc.lastname}
                    </div>
                </div>
                <div>
                    <label className="block text-xs font-bold text-casinoapp-black mb-1">
                        {t("auth:register.birthDate")}
                    </label>
                    <div className="h-6 flex items-center text-sm text-muted-foreground">
                        {formatDateLocal(kyc.birthDate, i18n.language)}
                    </div>
                </div>
                <div>
                    <label className="block text-xs font-bold text-casinoapp-black mb-1">
                        {t("auth:register.gender")}
                    </label>
                    <div className="h-6 flex items-center text-sm text-muted-foreground">
                        {t(`verification.identity.gender.${kyc.gender}`)}
                    </div>
                </div>
                <div>
                    <label className="block px-1 text-xs font-bold text-casinoapp-black mb-1">
                        {t("auth:register.documentType")}
                    </label>
                    <div className="h-6 flex items-center text-sm text-muted-foreground">
                        {t(`verification.identity.documentType.${kyc.documentType}`)}
                    </div>
                </div>
                <div>
                    <label className="block text-xs font-bold text-casinoapp-black mb-1">
                        {t("auth:register.documentNumber")}
                    </label>
                    <div className="h-6 flex items-center text-sm text-muted-foreground">
                        {kyc.documentNumber}
                    </div>
                </div>

            </div>


        </CollapsibleCard>
    );
};

export default IdentityInfoCard;