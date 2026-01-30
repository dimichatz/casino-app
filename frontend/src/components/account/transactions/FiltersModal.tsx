import {Card, CardContent} from "@/components/ui/card.tsx";
import {X} from "lucide-react";
import {useTranslation} from "react-i18next";
import { useUI } from "@/hooks/useUI";
import {useEffect, useState} from "react";
import {defaultFilters, useTransactionFilters} from "@/context/TransactionFiltersContext.tsx";
import {Checkbox} from "@/components/ui/checkbox";
import { Calendar } from "@/components/ui/calendar";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Button } from "@/components/ui/button";
import { format } from "date-fns";
import { CalendarIcon } from "lucide-react";

const transactionTypeKeys = [
    "includeDeposits",
    "includeWithdrawals",
    "includeCasino",
    "includeOther",
] as const;

const FiltersModal = () => {
    const { t } = useTranslation("account");
    const { activeModal, closeModal } = useUI();

    const { filters, setFilters } = useTransactionFilters();

    const [draft, setDraft] = useState(filters);

    //calendar
    const today = new Date();
    const startDate = draft.dateStart ? new Date(draft.dateStart) : undefined;
    const endDate = draft.dateEnd ? new Date(draft.dateEnd) : undefined;
    const [startOpen, setStartOpen] = useState(false);
    const [endOpen, setEndOpen] = useState(false);

    useEffect(() => {
        if (activeModal === "transaction-filters") {
            setDraft(defaultFilters);
        }
    }, [activeModal]);

    const resetFilters = () => {
        setDraft(defaultFilters);
    };

    if (activeModal !== "transaction-filters") return null;

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
                                    {t("transactions.filters.title")}
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
                                    {t("transactions.filters.dateDescription")}
                                </span>
                            </div>

                            <div className="grid grid-cols-2 gap-8">
                                <div role="group" aria-labelledby="startDateLabel">
                                    <Popover open={startOpen} onOpenChange={setStartOpen}>
                                        <PopoverTrigger asChild>
                                            <Button
                                                variant="outline"
                                                className="w-full justify-between font-normal"
                                            >
                                                <span>
                                                    {startDate
                                                        ? format(startDate, "dd-MM-yyyy")
                                                        : t("transactions.filters.startDate")}
                                                </span>
                                                <CalendarIcon className="h-4 w-4 opacity-70" />
                                            </Button>
                                        </PopoverTrigger>

                                        <PopoverContent className="w-[var(--radix-popover-trigger-width)] p-0 " align="start">
                                            <Calendar
                                                mode="single"
                                                className="w-full"
                                                selected={startDate}
                                                disabled={{ after: today }}
                                                onSelect={(date) => {
                                                    setDraft(prev => ({
                                                        ...prev,
                                                        dateStart: date ? format(date, "yyyy-MM-dd") : null,
                                                    }));

                                                    setStartOpen(false);
                                                }}
                                            />
                                        </PopoverContent>
                                    </Popover>
                                </div>

                                <div role="group" aria-labelledby="endDateLabel">
                                    <Popover open={endOpen} onOpenChange={setEndOpen}>
                                        <PopoverTrigger asChild>
                                            <Button
                                                variant="outline"
                                                className="w-full justify-between font-normal"
                                                disabled={!startDate}
                                            >
                                                <span>
                                                    {endDate
                                                        ? format(endDate, "dd-MM-yyyy")
                                                        : t("transactions.filters.endDate")}
                                                </span>
                                                <CalendarIcon className="h-4 w-4 opacity-70" />
                                            </Button>
                                        </PopoverTrigger>

                                        <PopoverContent
                                            align="start"
                                            className="w-[var(--radix-popover-trigger-width)] p-0"
                                        >
                                            <Calendar
                                                className="w-full"
                                                mode="single"
                                                selected={endDate}
                                                disabled={{
                                                    after: today,
                                                    before: startDate,
                                                }}
                                                onSelect={(date) => {
                                                    if (!date) return;

                                                    const end = new Date(date);
                                                    end.setHours(23, 59, 59, 999);

                                                    setDraft(prev => ({
                                                        ...prev,
                                                        dateEnd: end.toISOString(),
                                                    }));

                                                    setEndOpen(false);
                                                }}
                                            />
                                        </PopoverContent>
                                    </Popover>
                                </div>
                            </div>

                            <div className="pt-4">
                                <span className="text-sm text-zinc-700">
                                    {t("transactions.filters.transactionTypeDescription")}
                                </span>
                            </div>

                            <div className="grid grid-cols-2 gap-8">
                                {transactionTypeKeys.map((key) => {
                                    const checkboxId = `transaction-type-${key}`;

                                    return (
                                        <div key={key} className="flex items-center gap-2">
                                            <Checkbox
                                                id={checkboxId}
                                                className="h-6 w-6 border-zinc-400
                                                data-[state=checked]:bg-casinoapp-dark-blue
                                                data-[state=checked]:text-white cursor-pointer"
                                                checked={draft[key]}
                                                onCheckedChange={(checked) =>
                                                    setDraft((prev) => ({
                                                        ...prev,
                                                        [key]: checked === true,
                                                    }))
                                                }
                                            />

                                            <label
                                                htmlFor={checkboxId}
                                                className="text-md font-medium cursor-pointer"
                                            >
                                                {t(`transactions.filters.${key}`)}
                                            </label>
                                        </div>
                                    );
                                })}

                            </div>

                            <div className="flex justify-between pt-4">
                                <button
                                    type="button"
                                    onClick={() => {
                                        resetFilters();
                                    }}
                                    className="border bg-casinoapp-light-gray/70 text-center p-2
                                    w-35 rounded-md text-casinoapp-black font-semibold cursor-pointer
                                    hover:opacity-90 disabled:opacity-50 disabled:cursor-default"
                                >
                                    {t("transactions.filters.clearButton")}
                                </button>

                                <button
                                    type="button"
                                    onClick={() => {
                                        setFilters(draft);
                                        closeModal();
                                    }}
                                    className="border bg-casinoapp-orange text-center p-2
                                    w-35 rounded-md text-casinoapp-black font-semibold cursor-pointer
                                    hover:opacity-90 disabled:opacity-50 disabled:cursor-default"
                                >
                                    {t("transactions.filters.applyButton")}
                                </button>
                            </div>

                        </CardContent>
                    </Card>
                </div>
            </div>
        </>
    );
};

export default FiltersModal;