import CollapsibleCard from "@/components/account/profile/player-protection/CollapsibleCard.tsx";
import {useTranslation} from "react-i18next";
import DepositLimitCard from "@/components/account/profile/player-protection/DepositLimitCard.tsx";
import type {PlayerFullDetails} from "@/types/player-read-api.ts";
import {getPlayer} from "@/services/api.player.ts";
import {useEffect, useState} from "react";
import LossLimitCard from "@/components/account/profile/player-protection/LossLimitCard.tsx";

const BettingLimitsCard = () => {
    const { t } = useTranslation("profile");

    const [player, setPlayer] = useState<PlayerFullDetails | null>(null);

    const loadPlayer = async () => {
        const data = await getPlayer();
        setPlayer(data);
    };

    useEffect(() => {
            void loadPlayer();
    }, []);


    const limits = player?.selfExclusionAndLimitDetails;

    return (
        <CollapsibleCard
            title={t("protection.limits.title")}
            rounded="top"
            defaultOpen
        >

                <div className="flex flex-col items-center pt-4 gap-4">
                    <DepositLimitCard
                        onUpdated={loadPlayer}
                        depositDailyLimit={limits?.depositDailyLimit ?? undefined}
                        depositWeeklyLimit={limits?.depositWeeklyLimit ?? undefined}
                        depositMonthlyLimit={limits?.depositMonthlyLimit ?? undefined}
                        pendingDepositDailyLimit={limits?.pendingDepositDailyLimit ?? undefined}
                        pendingDepositDailyLimitStart={limits?.pendingDepositDailyLimitStart ?? undefined}
                        pendingDepositWeeklyLimit={limits?.pendingDepositWeeklyLimit ?? undefined}
                        pendingDepositWeeklyLimitStart={limits?.pendingDepositWeeklyLimitStart ?? undefined}
                        pendingDepositMonthlyLimit={limits?.pendingDepositMonthlyLimit ?? undefined}
                        pendingDepositMonthlyLimitStart={limits?.pendingDepositMonthlyLimitStart ?? undefined}
                    />
                    <LossLimitCard
                        onUpdated={loadPlayer}
                        lossDailyLimit={limits?.lossDailyLimit ?? undefined}
                        lossWeeklyLimit={limits?.lossWeeklyLimit ?? undefined}
                        lossMonthlyLimit={limits?.lossMonthlyLimit ?? undefined}
                        pendingLossDailyLimit={limits?.pendingLossDailyLimit ?? undefined}
                        pendingLossDailyLimitStart={limits?.pendingLossDailyLimitStart ?? undefined}
                        pendingLossWeeklyLimit={limits?.pendingLossWeeklyLimit ?? undefined}
                        pendingLossWeeklyLimitStart={limits?.pendingLossWeeklyLimitStart ?? undefined}
                        pendingLossMonthlyLimit={limits?.pendingLossMonthlyLimit ?? undefined}
                        pendingLossMonthlyLimitStart={limits?.pendingLossMonthlyLimitStart ?? undefined}
                    />
                </div>

        </CollapsibleCard>
    );
};

export default BettingLimitsCard;