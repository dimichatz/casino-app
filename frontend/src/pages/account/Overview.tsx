import {Card, CardContent} from "@/components/ui/card.tsx";
import {Button} from "@/components/ui/button.tsx";
import {usePlayer} from "@/hooks/usePlayer.ts";
import {Euro, Wallet} from "lucide-react";
import {useTranslation} from "react-i18next";
import {useAuth} from "@/hooks/useAuth.ts";
import {useNavigate} from "react-router-dom";
import {useDocumentTitle} from "@/hooks/useDocumentTitle.ts";

const Overview = () => {
    const navigate = useNavigate();
    const { t } = useTranslation("account");
    useDocumentTitle(t("common:documentTitle.overview"));
    const { balance } = usePlayer();
    const { username } = useAuth();

    return (
        <>
            <div className="flex flex-col text-casinoapp-black">
                <span className="text-2xl font-bold text-white p-6">
                    {t("overview.title")}
                </span>

                <Card className="w-full rounded-2xl shadow-sm shadow-casinoapp-light-gray">
                    <CardContent className="space-y-8 p-10">

                            <div>
                                <p>
                                    {t("overview.greeting")}
                                    <span className="font-semibold">
                                        {username}
                                    </span>
                                </p>
                            </div>

                        <div
                            className="rounded-xl bg-casinoapp-light-gray/30 py-10 px-6 lg:px-36
                             flex flex-col text-center gap-2"
                        >
                            <span
                                className="text-sm text-casinoapp-dark-blue flex
                                items-center justify-center gap-1"
                            >
                                <span
                                    className="flex items-center justify-center w-10 h-10
                                     rounded-full bg-casinoapp-light-blue/20"
                                >
                                    <Wallet className="w-6 h-6" />
                                </span>
                                {t("overview.balance")}
                            </span>

                            <p className="mt-2 text-3xl font-bold flex items-center justify-center gap-2">
                                {balance === null ? (
                                    <div className="flex items-center animate-pulse">
                                        <div className="h-9 w-24 rounded-md bg-muted-foreground/20" />
                                    </div>
                                ) : (
                                    <>
                                        {balance.toLocaleString("el-GR", {
                                            minimumFractionDigits: 2,
                                            maximumFractionDigits: 2,
                                        })}
                                        <Euro />
                                    </>
                                )}
                            </p>
                        </div>

                        <div className="flex gap-10">
                            <Button
                                onClick={() => navigate("/account/withdraw")}
                                className="flex-1 font-extrabold text-white tracking-wider
                                 bg-casinoapp-black cursor-pointer"

                            >
                                {t("overview.withdrawButton")}
                            </Button>

                            <Button
                                onClick={() => navigate("/account/deposit")}
                                className="flex-1 font-extrabold text-casinoapp-black
                                bg-casinoapp-light-lime-green
                                hover:bg-casinoapp-lime-green
                                cursor-pointer"
                            >
                                {t("overview.depositButton")}
                            </Button>
                        </div>

                    </CardContent>
                </Card>
            </div>
        </>
    );
};

export default Overview;