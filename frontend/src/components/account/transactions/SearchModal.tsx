import {Card, CardContent} from "@/components/ui/card.tsx";
import {Search, X} from "lucide-react";
import {useTranslation} from "react-i18next";
import { useUI } from "@/hooks/useUI";
import { useTransactionFilters } from "@/context/TransactionFiltersContext.tsx";
import {useEffect, useState} from "react";

const SearchModal = () => {
    const { t } = useTranslation("account");
    const { activeModal, closeModal } = useUI();
    const { setFilters } = useTransactionFilters();

    const [transactionNumber, setTransactionNumberInput] = useState("");

    useEffect(() => {
        if (activeModal === "transaction-search") {
            setTransactionNumberInput("");
        }
    }, [activeModal]);

    if (activeModal !== "transaction-search") return null;

    return (
        <>
            <div className="fixed inset-0 z-50 flex items-center justify-center">
                <div
                    className="absolute inset-0 bg-black/70"
                    onClick={closeModal}
                />
                <div
                    onClick={(e) => e.stopPropagation()}
                    className="relative w-full max-w-md"
                >
                    <Card className="mb-4 rounded-2xl">
                        <CardContent className="space-y-6 pt-4">
                            <div className="relative flex items-center justify-center border-b-2 pb-2">
                                    <span className="text-lg font-bold text-casinoapp-black">
                                        {t("transactions.search.innerTitle")}
                                     </span>

                                <button
                                    type="button"
                                    onClick={closeModal}
                                    className="absolute right-0 pr-0.5 text-zinc-400 hover:text-zinc-600"
                                >
                                    <X aria-hidden="true" />
                                </button>
                            </div>

                            <div>
                                    <span className="text-sm text-zinc-700">
                                        {t("transactions.search.description")}
                                     </span>
                            </div>

                            <div>
                                <label
                                    htmlFor="transactionNumber"
                                    className="text-sm font-medium"
                                >

                                    {t("transactions.search.transactionId")}
                                </label>
                                <div className="relative w-full">
                                    <input
                                        id="transactionNumber"
                                        type="number"
                                        value={transactionNumber}
                                        onChange={(e) =>
                                            setTransactionNumberInput(e.target.value)
                                        }
                                        className="mt-1 w-full rounded-md border px-3 py-2 "

                                    />
                                    <Search
                                        className="absolute mt-0.5 right-3 top-1/2
                                                -translate-y-1/2 text-zinc-400 h-6 w-6"
                                    />
                                </div >
                            </div>

                            <div className="flex justify-end">
                                <button
                                    type="button"
                                    onClick={() => {
                                        setFilters(prev => ({
                                            ...prev,
                                            transactionNumber: transactionNumber.trim() || null,
                                        }));
                                        closeModal();
                                    }}
                                    className="border bg-casinoapp-orange text-center p-2
                                            w-30 rounded-md text-casinoapp-black font-semibold cursor-pointer
                                            hover:opacity-90 disabled:opacity-50 disabled:cursor-default"
                                >
                                    {t("transactions.search.searchButton")}
                                </button>
                            </div>
                        </CardContent>
                    </Card>
                </div>
            </div>
        </>
    )
}

export default SearchModal;