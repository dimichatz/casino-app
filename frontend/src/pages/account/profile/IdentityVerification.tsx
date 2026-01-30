import {useDocumentTitle} from "@/hooks/useDocumentTitle.ts";
import {useTranslation} from "react-i18next";
import IdentityInfoCard from "@/components/account/profile/verification/IdentityInfoCard.tsx";
import {useEffect, useState} from "react";
import type {PlayerFullDetails} from "@/types/player-read-api.ts";
import {getPlayer} from "@/services/api.player.ts";
import {CircleCheck} from "lucide-react";
import {Card} from "@/components/ui/card.tsx";
import UploadDocumentCard from "@/components/account/profile/verification/UploadDocumentCard.tsx";

const IdentityVerification = () => {
    const { t } = useTranslation("profile");
    useDocumentTitle(t("common:documentTitle.verification"));

    const [player, setPlayer] = useState<PlayerFullDetails | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    const loadPlayer = async () => {
        const data = await getPlayer();
        setPlayer(data);
    };

    useEffect(() => {
        const init = async () => {
            try {
                await loadPlayer();
            } finally {
                setIsLoading(false);
            }
        };
        void init();
    }, []);

    if (isLoading || !player) {
        return null;
    }

    const kycDetails = player.kycDetails;


    return (
        <>
            <div className="flex flex-col text-casinoapp-black w-full max-w-2xl">
                <span className="text-2xl font-bold text-white p-6">
                    {t("verification.title")}
                </span>
                <div className="flex flex-col p-4 w-full rounded-xl bg-white/95 gap-4 select-none">
                    <section className="px-6 py-2 text-casinoapp-black">
                        <p className="text-md font-semibold">
                            {t("verification.description")}
                        </p>

                        <p className="text-sm text-casinoapp-muted mt-1">
                            {t("verification.sub-description")}
                        </p>
                    </section>

                    <IdentityInfoCard kyc={kycDetails} />

                    <div>
                        {kycDetails?.isKycVerified ? (
                            <Card
                                className="flex w-full h-auto
                                 items-center px-4 py-6 gap-4
                                 border-zinc-300 rounded-2xl
                                 border-l-4 border-l-green-500
                                 text-base font-semibold"
                            >
                                <CircleCheck className="h-10 w-10 text-green-500" />
                                <p>{t("verification.verified")}</p>
                            </Card>
                        ) : (
                            <UploadDocumentCard />
                        )}
                    </div>
                </div>
            </div>
        </>
    );
};

export default IdentityVerification;
