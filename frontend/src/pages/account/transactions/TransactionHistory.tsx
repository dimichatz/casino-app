import {Card, CardContent} from "@/components/ui/card.tsx";
import {useTranslation} from "react-i18next";
import {useCallback, useEffect, useRef, useState} from "react";
import type {TransactionReadOnlyDTO} from "@/types/transactions-api.ts";
import {transactionService} from "@/services/api.transactions.ts";
import TransactionRow from "@/components/account/transactions/TransactionRow.tsx";
import TransactionRowSkeleton from "@/components/account/transactions/TransactionRowSkeleton.tsx";
import {Download, SlidersHorizontal} from 'lucide-react';
import { useUI } from "@/hooks/useUI.ts";
import {useTransactionFilters} from "@/context/TransactionFiltersContext.tsx";
import {toast} from "sonner";
import {useDocumentTitle} from "@/hooks/useDocumentTitle.ts";

const PAGE_SIZE = 10;

const TransactionHistory = () => {
    const { t } = useTranslation("account");
    useDocumentTitle(t("common:documentTitle.history"));
    const {openTransactionSearch, openTransactionFilters} = useUI();
    const { filters } = useTransactionFilters();

    const [transactions, setTransactions] = useState<TransactionReadOnlyDTO[]>([]);
    const [loading, setLoading] = useState(false);
    const [hasMore, setHasMore] = useState(true);

    const observerRef = useRef<HTMLDivElement | null>(null);
    const scrollRootRef = useRef<HTMLDivElement | null>(null);

    const pageRef = useRef(1);
    const fetchingRef = useRef(false);

    const sleep = (ms: number) =>
        new Promise(resolve => setTimeout(resolve, ms));

    const fetchTransactions = useCallback(async () => {
        if (fetchingRef.current || !hasMore) return;

        fetchingRef.current = true;
        setLoading(true);

        try {
            await sleep(600);
            const res = await transactionService.getTransactions({
                pageNumber: pageRef.current,
                pageSize: PAGE_SIZE,
                transactionNumber: filters.transactionNumber ?? undefined,
                dateStart: filters.dateStart ?? undefined,
                dateEnd: filters.dateEnd ?? undefined,
                includeDeposits: filters.includeDeposits,
                includeWithdrawals: filters.includeWithdrawals,
                includeCasino: filters.includeCasino,
                includeOther: filters.includeOther,
            });

            setTransactions(prev => [...prev, ...res.data]);

            setHasMore(res.pageNumber < res.totalPages);
            pageRef.current += 1;
        } catch (err: unknown) {
            console.error("Failed to fetch transactions", err);
            toast.error(t("errors.generic"));
            setHasMore(false);
        } finally {
            fetchingRef.current = false;
            setLoading(false);
        }
    }, [hasMore, filters, t]);

    // reset when filter changes
    useEffect(() => {
        pageRef.current = 1;
        setTransactions([]);
        setHasMore(true);
    }, [filters]);

    useEffect(() => {
        void fetchTransactions();
    }, [filters, fetchTransactions]);

    useEffect(() => {
        if (!observerRef.current || !hasMore || !scrollRootRef.current) return;

        const observer = new IntersectionObserver(
            entries => {
                if (entries[0].isIntersecting) {
                    void fetchTransactions();
                }
            },
            {
                root: scrollRootRef.current,
                rootMargin: "200px",
                threshold: 0,
            }
        );

        observer.observe(observerRef.current);
        return () => observer.disconnect();
    }, [fetchTransactions, hasMore]);

    const handleDownload = async () => {
        try {
            if (transactions.length === 0) {
                return;
            }

            const blob = await transactionService.downloadTransactions(filters);

            const url = window.URL.createObjectURL(blob);
            const link = document.createElement("a");
            link.href = url;
            link.download = "transactions.csv";

            document.body.appendChild(link);
            link.click();

            link.remove();
            window.URL.revokeObjectURL(url);
        } catch (err) {
            console.error("Failed to download transactions", err);
            toast.error(t("transactions.errors.downloadFailed"));
        }
    };

    return (
        <>
            <div className="flex flex-col text-casinoapp-black">
                <span className="text-2xl font-bold text-white p-6">
                    {t("transactions.title")}
                </span>

                <div className="p-4 flex justify-between">
                    <button
                        type="button"
                        onClick={openTransactionSearch}
                        className="rounded-xs px-2 pt-4 border-b-3 border-transparent text-white font-semibold cursor-pointer
                        bg-transparent hover:border-b-3 hover:border-casinoapp-light-blue"
                    >
                        {t("transactions.search.title")}
                    </button>

                    <div className="flex items-center gap-3">
                        <button
                            type="button"
                            onClick={openTransactionFilters}
                            className="flex items-center justify-center rounded-md h-10 w-10 cursor-pointer
                         text-white bg-casinoapp-light-blue hover:bg-casinoapp-light-blue/90"
                        >
                            <SlidersHorizontal className="h-6 w-6" />
                        </button>

                        <button
                            type="button"
                            onClick={handleDownload}
                            disabled={transactions.length === 0}
                            className={`
                                flex items-center justify-center rounded-md h-10 w-10
                                ${transactions.length === 0
                                ? "bg-zinc-300 text-zinc-500"
                                : "bg-white text-casinoapp-black hover:bg-zinc-300 cursor-pointer"}
  `}
                        >
                            <Download  className="h-6 w-6" />
                        </button>
                    </div>

                </div>

                <Card className="w-full rounded-xl shadow-sm shadow-casinoapp-light-gray pt-2">
                    <CardContent
                        ref={scrollRootRef}
                        className="space-y-2 px-2 max-h-[70vh]
                        scrollbar-rounded overflow-y-auto">
                        {transactions.map(tx => (
                            <TransactionRow key={tx.id} tx={tx} />
                        ))}

                        {loading &&
                            Array.from({ length: PAGE_SIZE }).map((_, i) => (
                                <TransactionRowSkeleton key={`skeleton-${i}`} />
                            ))}

                        {hasMore && <div ref={observerRef} className="h-1" />}

                        {!hasMore && transactions.length > 0 && (
                            <div className="py-4 text-center text-xs text-zinc-600">
                                End of transaction history
                            </div>
                        )}

                        {!loading && transactions.length === 0 && (
                            <div className="grid lg:grid-cols-[1fr_6fr_1fr] pt-2 text-center text-sm text-casinoapp-black">
                                <span className="hidden lg:block w-32"/>
                                {t("transactions.noTransactionsFound")}
                                <span className="hidden lg:block w-32"/>
                            </div>
                        )}

                    </CardContent>
                </Card>
            </div>
        </>
    );
};

export default TransactionHistory;