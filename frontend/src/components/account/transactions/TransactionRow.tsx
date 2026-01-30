import { cn } from "@/lib/utils";
import type {TransactionReadOnlyDTO} from "@/types/transactions-api";
import {
    BanknoteArrowDown,
    BanknoteArrowUp,
    Dices,
    CircleDot
} from "lucide-react";
import { ChevronDown, ChevronUp } from "lucide-react";
import {type ElementType, useState} from "react";
import {useTranslation} from "react-i18next";
import {TRANSACTION_STATUS_I18N_KEY, TRANSACTION_TYPE_I18N_KEY} from "@/i18n/enumMappings.ts";

interface Props {
    tx: TransactionReadOnlyDTO;
}

type TxIconConfig = {
    Icon: ElementType;
    iconColor: string;
};

const TRANSACTION_ICON_MAP: Record<
    TransactionReadOnlyDTO["transactionType"],
    TxIconConfig
> = {
    Deposit: {
        Icon: BanknoteArrowUp,
        iconColor: "text-green-700",
    },
    Withdraw: {
        Icon: BanknoteArrowDown,
        iconColor: "text-red-700",
    },
    Bet: {
        Icon: Dices,
        iconColor: "text-blue-800",
    },
    Win: {
        Icon: Dices,
        iconColor: "text-blue-800",
    },
    Tax: {
        Icon: Dices,
        iconColor: "text-blue-800",
    },
    Bonus: {
        Icon: CircleDot,
        iconColor: "text-orange-400",
    }
};

const NEGATIVE_TRANSACTION_TYPES: TransactionReadOnlyDTO["transactionType"][] = [
    "Withdraw",
    "Bet",
    "Tax",
];

const TransactionRow = ({ tx }: Props) => {
    const { t } = useTranslation("account");
    const isNegative = NEGATIVE_TRANSACTION_TYPES.includes(tx.transactionType);

    const amountSign = isNegative ? "-" : "+";
    const amountColor = isNegative ? "text-red-500" : "text-green-600";

    const { Icon, iconColor } =
    TRANSACTION_ICON_MAP[tx.transactionType] ?? {
        Icon: CircleDot,
        iconColor: "text-orange-400",
    };

    const [expanded, setExpanded] = useState(false);

    return (
        <div
            className="grid lg:grid-cols-[2fr_1fr_1fr_1fr_1fr_auto] grid-cols-[2fr_1fr] items-center gap-x-12
             rounded-lg bg-white p-4 text-sm text-casinoapp-black shadow-sm"
        >
            <div className="grid grid-cols-[1fr_2fr] items-center">
                <div className="flex h-8 w-8 items-center justify-center rounded-full bg-zinc-100">
                    <Icon className={cn("h-4 w-4", iconColor)} />
                </div>

                <div className="flex flex-col overflow-hidden">
                <span className="font-medium truncate">
                    {t(TRANSACTION_TYPE_I18N_KEY[tx.transactionType])}
                </span>

                    {tx.gameName && (
                        <span className="text-xs text-zinc-600 truncate">
                        {tx.gameName}
                    </span>
                    )}
                </div>
            </div>


            <div className="hidden lg:block font-medium truncate">
                {t(TRANSACTION_STATUS_I18N_KEY[tx.transactionStatus])}
            </div>

            <div className="hidden lg:block text-zinc-600 truncate">
                ID: {tx.transactionNumber}
            </div>

            <div className="hidden lg:block text-zinc-600 truncate">
                {new Date(tx.insertedAt).toLocaleString("el-GR", {
                    day: "2-digit",
                    month: "2-digit",
                    year: "numeric",
                    hour: "2-digit",
                    minute: "2-digit",
                    hourCycle: "h23",
                }).replace(",", " -")}
            </div>

            <div className={cn("text-right font-semibold truncate", amountColor)}>
                {amountSign}
                {new Intl.NumberFormat("el-GR", {
                    style: "currency",
                    currency: "EUR",
                }).format(tx.amount)}
            </div>

            <button
                onClick={() => setExpanded(prev => !prev)}
                className="hidden lg:block items-center justify-center text-zinc-500 hover:text-zinc-800"
            >
                {expanded ? (
                    <ChevronUp className="h-4 w-4" />
                ) : (
                    <ChevronDown className="h-4 w-4" />
                )}
            </button>

            {expanded && (
                <div
                    className="col-span-full mt-4 grid grid-cols-3 gap-2
                rounded-md bg-casinoapp-light-gray/20 px-6 py-3
                text-xs text-casinoapp-black"
                >
                    <div>
                        <span className="font-medium text-zinc-600">{t("transactions.oldBalance")}</span>{" "}
                        {tx.oldBalance?.toFixed(2)} €
                    </div>

                    <div>
                        <span className="font-medium text-zinc-600">{t("transactions.newBalance")}</span>{" "}
                        {tx.newBalance?.toFixed(2)} €
                    </div>
                </div>
            )}
        </div>
    );
};

export default TransactionRow;
