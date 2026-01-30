import { Card } from "@/components/ui/card";
import { Play } from "lucide-react"
import {useUI} from "@/hooks/useUI.ts";
import {useAuth} from "@/hooks/useAuth.ts";
import {getGameAccess} from "@/services/api.player.ts";
import {toast} from "sonner";
import {useTranslation} from "react-i18next";

let gameWindow: Window | null = null;

type Props = {
    title: string;
    thumbnailUrl: string;
};

const GameCard = ({ title, thumbnailUrl }: Props) => {
    const { t } = useTranslation("games");
    const { isAuthenticated, accessToken } = useAuth();
    const { openLogin } = useUI();

    const handleClick = async () => {
        if (!isAuthenticated || !accessToken) {
            openLogin();
            return;
        }

        const access = await getGameAccess();

        if (!access.hasGameAccess) {
            toast.warning(
                t(`errors.gameAccess.${access.reason?.toLowerCase()}`)
            );
            return;
        }

        const gameUrl =
            `https://localhost:5001/games/highlow/index.html?token=${encodeURIComponent(accessToken)}`;

        const width = 960;
        const height = 600;

        const left =
            window.screenX +
            (window.outerWidth - width) / 2 -
            120;

        const top =
            window.screenY +
            (window.outerHeight - height) / 2;

        if (gameWindow && !gameWindow.closed) {
            gameWindow.focus();
        } else {
            gameWindow = window.open(
                gameUrl,
                "_blank",
                `width=${width},height=${height},left=${left},top=${top},resizable=yes`
            );
        }
    };

    return (
        <Card className="bg-transparent border-none shadow-none">

            <div
                onClick={handleClick}
                className="group relative overflow-hidden rounded-lg aspect-[16/9] cursor-pointer">
                <img
                    src={thumbnailUrl}
                    alt={title}
                    className="h-full w-full object-cover transition-transform duration-300 ease-out
                        group-hover:scale-120"
                />

                <div className="absolute inset-0 bg-black/40 opacity-0 transition-opacity duration-300
                    group-hover:opacity-100 flex items-center justify-center">

                    <Play className="h-12 w-12 text-white opacity-0 scale-90
                        transition-all duration-300 group-hover:opacity-100 group-hover:scale-100"
                          fill="white"/>
                </div>
            </div>

            <div className="mt-2 text-center text-sm text-casinoapp-light-gray">
                {title}
            </div>
        </Card>
    );
};

export default GameCard;