import { Card } from "@/components/ui/card";
import {useTranslation} from "react-i18next";
import robot from "@/assets/images/no_results_robot.png"

const NoGamesFoundCard = () => {
    const { t } = useTranslation("games");

    return (
        <Card className="bg-transparent border-none shadow-none">
            <div className="flex flex-col items-center">
                <img className="w-51 h-76" src={robot} alt="No games found"/>

                <p className="text-base text-white">
                    {t("noGameFoundTitle")}
                </p>
                <p className="text-sm text-casinoapp-light-gray">
                    {t("noGameFoundText")}
                </p>
            </div>

        </Card>
    );
};

export default NoGamesFoundCard;