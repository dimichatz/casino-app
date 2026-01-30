import {useTranslation} from "react-i18next";
import EditEmailCard from "@/components/account/profile/player-details/EditEmailCard.tsx";
import EditMobileCard from "@/components/account/profile/player-details/EditMobileCard.tsx";
import EditAddressCard from "@/components/account/profile/player-details/EditAddressCard.tsx";
import {useEffect, useState} from "react";
import type {PlayerFullDetails} from "@/types/player-read-api.ts";
import {getPlayer} from "@/services/api.player.ts";
import {useDocumentTitle} from "@/hooks/useDocumentTitle.ts";

const PersonalDetails = () => {
    const { t } = useTranslation("profile");
    useDocumentTitle(t("common:documentTitle.profile"));

    const [openRow, setOpenRow] = useState<"email" | "mobile" | "address" | null>(null);

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

    const userDetails = player.userDetails;
    const addressDetails = player.addressDetails;

    return (
        <>
            <div className="flex flex-col text-casinoapp-black w-full max-w-2xl">
                <span className="text-2xl font-bold text-white p-6">
                    {t("details.title")}
                </span>
                <EditEmailCard
                    onUpdated={loadPlayer}
                    email={userDetails?.email}
                    open={openRow === "email"}
                    disabled={openRow !== null && openRow !== "email"}
                    onToggle={() =>
                        setOpenRow((prev) => (prev === "email" ? null : "email"))
                    }
                />
                <EditMobileCard
                    onUpdated={loadPlayer}
                    phoneNumber={userDetails?.phoneNumber}
                    open={openRow === "mobile"}
                    disabled={openRow !== null && openRow !== "mobile"}
                    onToggle={() =>
                        setOpenRow((prev) => (prev === "mobile" ? null : "mobile"))
                    }
                />
                <EditAddressCard
                    onUpdated={loadPlayer}
                    address={addressDetails}
                    open={openRow === "address"}
                    disabled={openRow !== null && openRow !== "address"}
                    onToggle={() =>
                        setOpenRow((prev) => (prev === "address" ? null : "address"))
                    }
                />
            </div>
        </>
    );
};

export default PersonalDetails;
