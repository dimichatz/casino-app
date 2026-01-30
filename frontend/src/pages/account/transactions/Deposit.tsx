import {Card, CardContent} from "@/components/ui/card.tsx";
import {CircleX, Euro} from "lucide-react";
import {Button} from "@/components/ui/button.tsx";
import {useTranslation} from "react-i18next";
import {useState} from "react";
import {depositSchema} from "@/schemas/account/transactions/deposit.ts";
import {parseEuroAmount} from "@/schemas/account/transactions/amount.ts";
import {transactionService} from "@/services/api.transactions.ts";
import {TRANSACTION_ERROR_MAP} from "@/errors/transactionErrors.ts";
import {toast} from "sonner";
import {usePlayer} from "@/hooks/usePlayer.ts";
import axios from "axios";
import * as React from "react";
import {useDocumentTitle} from "@/hooks/useDocumentTitle.ts";

const Deposit = () => {
    const { t } = useTranslation("account");
    useDocumentTitle(t("common:documentTitle.deposit"));
    const [amount, setAmount] = useState("");
    const [error, setError] = useState<string | null>(null);
    const [touched, setTouched] = useState(false);
    const { balance, setBalance } = usePlayer();
    const [isSubmitting, setIsSubmitting] = useState(false);

    const formatEuro = (value: string) => {
        if (!value) return "";

        const [integer, decimals] = value.split(",");

        const formattedInteger = integer
            .replace(/^0+(?=\d)/, "") // remove leading zeros
            .replace(/\B(?=(\d{3})+(?!\d))/g, ".");

        return decimals !== undefined
            ? `${formattedInteger},${decimals}`
            : formattedInteger;
    };

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        let value = e.target.value;

        value = value.replace(/\./g, "");
        value = value.replace(/[^0-9,]/g, "");

        const parts = value.split(",");
        if (parts.length > 2) {
            value = parts[0] + "," + parts.slice(1).join("");
        }

        if (value.startsWith(",")) {
            value = "0" + value;
        }

        const [integer, decimals] = value.split(",");
        if (decimals && decimals.length > 2) {
            value = `${integer},${decimals.slice(0, 2)}`;
        }

        if (error) {
            setError(null)
        }

        setAmount(value);
    };

    const validate = () => {
        if (balance === null) {
            setError("errors.generic");
            return false;
        }
        const result = depositSchema.safeParse(amount)

        if (!result.success) {
            const errorKey = result.error.issues[0].message
            setError(errorKey)
            return false
        }

        setError(null)
        return true
    };

    const isDisabled =
        isSubmitting ||
        amount === "";

    const handleSubmit = async () => {
        setTouched(true)

        if (!validate()) return;

        setIsSubmitting(true);

        try {
            const response = await transactionService.deposit({
                amount: parseEuroAmount(amount),
            });

            if (response.newBalance !== null && response.newBalance !== undefined) {
                setBalance(response.newBalance);
            }

            setAmount("")
            setError(null)
            setTouched(false)
            toast.success(t("deposit.success"));
        } catch (err: unknown) {
            console.warn("Request rejected:", err);

            let backendCode: string | null = null;
            let key = "errors.generic";

            if (axios.isAxiosError(err)) {
                backendCode =
                    err.response?.data?.code ??
                    err.code ??
                    null;
            }

            if (backendCode && backendCode in TRANSACTION_ERROR_MAP) {
                key = TRANSACTION_ERROR_MAP[backendCode];
            }
            setError(key);
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <>
            <div className="flex flex-col text-casinoapp-black">
                <span className="text-2xl font-bold text-white p-6">
                    {t("deposit.title")}
                </span>

                <Card className="w-full rounded-2xl shadow-sm shadow-casinoapp-light-gray">
                    <CardContent className="space-y-2 p-10">

                        <div className="flex flex-col gap-2">
                            <div className="relative w-full">
                                <Euro className="absolute right-8 top-1/2 -translate-y-1/2 h-7 w-7 text-casinoapp-black" />
                                <input
                                    name="depositAmount"
                                    type="text"
                                    placeholder="0,00"
                                    value={formatEuro(amount)}
                                    onChange={handleChange}
                                    onBlur={() => {
                                        setTouched(true)
                                        validate()
                                    }}
                                    inputMode="decimal"
                                    pattern="[0-9,]*"
                                    className="w-[57%] text-4xl font-extrabold tracking-tight
                                    text-right focus-visible:outline-none appearance-none"
                                />
                            </div>

                            <div className="flex items-center justify-center ">
                            {touched && error && (
                                <>
                                    <CircleX className="h-4 w-4 text-red-500" />
                                    <p className="ml-1 text-red-500 text-xs font-semibold max-w-sm">
                                        {t(error)}
                                    </p>
                                </>
                            )}
                            </div>
                        </div>

                        <div className="flex">

                            <Button
                                onClick={handleSubmit}
                                disabled={isDisabled}
                                className="flex-1 font-extrabold text-casinoapp-black
                                bg-casinoapp-light-lime-green
                                hover:bg-casinoapp-lime-green
                                cursor-pointer mt-2"
                            >
                                {t("deposit.depositButton")}
                            </Button>
                        </div>

                    </CardContent>
                </Card>
            </div>
        </>
    );
};

export default Deposit;