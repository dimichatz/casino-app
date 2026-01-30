import {Card, CardContent, CardHeader, CardTitle} from "@/components/ui/card.tsx";
import {Info, TrendingDown} from "lucide-react";
import {
    Tooltip, TooltipContent,
    TooltipProvider, TooltipTrigger,
} from "@/components/ui/tooltip";
import {
    Select, SelectContent, SelectItem,
    SelectTrigger, SelectValue,
} from "@/components/ui/select";
import {Button} from "@/components/ui/button.tsx";
import {Slider} from "@/components/ui/slider.tsx";
import {useTranslation} from "react-i18next";
import * as React from "react";
import {updatePlayer} from "@/services/api.player.ts";
import {toast} from "sonner";

interface PlayerUpdateLimitDetails {
    lossDailyLimit?: number;
    lossWeeklyLimit?: number;
    lossMonthlyLimit?: number;
}

interface LossLimitCardProps {
    lossDailyLimit?: number;
    lossWeeklyLimit?: number;
    lossMonthlyLimit?: number;
    pendingLossDailyLimit?: number;
    pendingLossDailyLimitStart?: string;
    pendingLossWeeklyLimit?: number;
    pendingLossWeeklyLimitStart?: string;
    pendingLossMonthlyLimit?: number;
    pendingLossMonthlyLimitStart?: string;
    onUpdated: () => Promise<void>;
}

const LossLimitCard = ({
                              onUpdated,
                              lossDailyLimit,
                              lossWeeklyLimit,
                              lossMonthlyLimit,
                              pendingLossDailyLimit,
                              pendingLossDailyLimitStart,
                              pendingLossWeeklyLimit,
                              pendingLossWeeklyLimitStart,
                              pendingLossMonthlyLimit,
                              pendingLossMonthlyLimitStart,
                          }: LossLimitCardProps) => {
    const MIN = 0
    const MAX = 10000
    const STEP = 10
    const DEFAULT_PERIOD = "daily";
    const DEFAULT_VALUE = 200;

    const euroFormatter = new Intl.NumberFormat("el-GR", {
        style: "currency",
        currency: "EUR",
        maximumFractionDigits: 0,
    });
    const { t } = useTranslation("profile");

    const pendingLimit =
        pendingLossDailyLimit != null
            ? { period: t("protection.limits.myLimits.day"),
                value: pendingLossDailyLimit,
                start: pendingLossDailyLimitStart }
            : pendingLossWeeklyLimit != null
                ? { period: t("protection.limits.myLimits.week"),
                    value: pendingLossWeeklyLimit,
                    start: pendingLossWeeklyLimitStart }
                : pendingLossMonthlyLimit != null
                    ? { period: t("protection.limits.myLimits.month"),
                        value: pendingLossMonthlyLimit,
                        start: pendingLossMonthlyLimitStart }
                    : null;

    const [value, setValue] = React.useState([DEFAULT_VALUE]);
    const [period, setPeriod] = React.useState<string>(DEFAULT_PERIOD);
    const [isEditing, setIsEditing] = React.useState(false);
    const [isSaving, setIsSaving] = React.useState(false);

    const currentLimit =
        period === "daily"
            ? lossDailyLimit
            : period === "weekly"
                ? lossWeeklyLimit
                : lossMonthlyLimit;

    const isDecrease =
        currentLimit != null && value[0] < currentLimit;
    const isIncrease =
        currentLimit != null && value[0] > currentLimit;
    const isSame =
        currentLimit != null && value[0] === currentLimit;

    const isSaveChangesDisabled = value[0] === 0 || !period || isSaving || isSame;

    const getPercentage = (value: number, min: number, max: number) =>
        ((value - min) / (max - min)) * 100

    const buildLimitDetails = (): PlayerUpdateLimitDetails => {
        switch (period) {
            case "daily":
                return { lossDailyLimit: value[0] };
            case "weekly":
                return { lossWeeklyLimit: value[0] };
            case "monthly":
                return { lossMonthlyLimit: value[0] };
            default:
                return {};
        }
    };

    const hasPending = !!pendingLimit;

    const onSave = async () => {
        setIsSaving(true);
        try {
            const limitDetails = buildLimitDetails();

            await updatePlayer({
                limitDetails: limitDetails,
            });
            await onUpdated?.();

            setValue([DEFAULT_VALUE]);
            setPeriod(DEFAULT_PERIOD);
            setIsEditing(false);
            toast.success(t("protection.limits.lossSuccess"));

        } catch (err: unknown) {
            console.warn("Request rejected:", err);
            toast.error(t("exclusion.errors.generic"));
        } finally {
            setIsSaving(false);
        }
    };

    const onEdit = () => {
        setValue([...value]);
        setPeriod(period);
        setIsEditing(true);
    };

    const onReset = () => {
        setValue([DEFAULT_VALUE]);
        setPeriod(DEFAULT_PERIOD);
        setIsEditing(false);
    };

    const newLimitPreview = () => {
        if (currentLimit == null) return null;

        const periodLabel =
            period === "daily"
                ? t("protection.limits.myLimits.day")
                : period === "weekly"
                    ? t("protection.limits.myLimits.week")
                    : t("protection.limits.myLimits.month");

        if (isDecrease) {
            return (
                <>
                <span className="font-medium">
                    {t("protection.limits.myLimits.newLoss")}{" "}
                </span>
                    {euroFormatter.format(value[0])} {periodLabel},{" "}
                    {t("protection.limits.myLimits.applyInstant")}
                </>
            );
        }

        if (isIncrease) {
            return (
                <>
                <span className="font-medium">
                    {t("protection.limits.myLimits.newLoss")}{" "}
                </span>
                    {euroFormatter.format(value[0])} {periodLabel},{" "}
                    {t("protection.limits.myLimits.applyPending")}
                </>
            );
        }

        return null;
    };

    return (
        <Card className="w-full">
            <CardHeader className="pb-6">
                <div className="flex items-start justify-between gap-4">
                    <div className="flex items-center gap-3">
                        <div
                            className="h-10 w-10 rounded-full border flex items-center justify-center
                            bg-casinoapp-dark-blue"
                        >
                            <TrendingDown className="h-5 w-5 text-white bg" />
                        </div>
                        <CardTitle className="text-lg select-none">
                            {t("protection.limits.lossTitle")}
                        </CardTitle>
                    </div>

                    <TooltipProvider>
                        <Tooltip>
                            <TooltipTrigger asChild>
                                <button
                                    type="button"
                                    className="inline-flex h-9 w-9 items-center justify-center rounded-full hover:bg-muted"
                                    aria-label="loss limit info"
                                >
                                    <Info className="h-5 w-5 text-zinc-700" />
                                </button>
                            </TooltipTrigger>
                            <TooltipContent className="max-w-xs text-xs mr-6">
                                {t("protection.limits.lossTool")}
                            </TooltipContent>
                        </Tooltip>
                    </TooltipProvider>
                </div>
            </CardHeader>

            <CardContent>
                <div className="flex flex-col w-full gap-12">
                    <div>
                        <label className="text-sm font-medium">
                            {t("protection.limits.maxAmountLabelLoss")}
                        </label>

                        <Select
                            value={period}
                            onValueChange={setPeriod}
                            disabled={!isEditing}
                        >
                            <SelectTrigger
                                className="
                                cursor-pointer disabled:cursor-default
                                ring-0 ring-offset-0 focus:ring-0
                                border-border focus:border-transparent
                                data-[state=open]:ring-0
                                data-[state=open]:outline-none
                                data-[state=open]:border-zinc-400
                                mt-2 select-none"
                            >
                                <SelectValue placeholder={t("protection.limits.selectPlaceholder")} />
                            </SelectTrigger>
                            <SelectContent>
                                <SelectItem value="daily">{t("protection.limits.myLimits.daily")}</SelectItem>
                                <SelectItem value="weekly">{t("protection.limits.myLimits.weekly")}</SelectItem>
                                <SelectItem value="monthly">{t("protection.limits.myLimits.monthly")}</SelectItem>
                            </SelectContent>
                        </Select>
                    </div >

                    <div className="relative pt-6">
                        <div
                            className="absolute -top-2 -translate-x-1/2 text-xs
                            font-semibold text-zinc-700 select-none"
                            style={{
                                left: `${getPercentage(value[0], MIN, MAX)}%`,
                            }}
                        >
                            {euroFormatter.format(value[0])}
                        </div>

                        <Slider
                            min={MIN}
                            max={MAX}
                            step={STEP}
                            value={value}
                            onValueChange={setValue}
                            disabled={!isEditing}
                            className={`
                            [&_[role=slider]]:focus-visible:outline-none
                            [&_[role=slider]]:focus-visible:ring-0
                            [&_[role=slider]]:focus-visible:ring-offset-0s
                            [&_[role=slider]]:bg-casinoapp-dark-blue
                            data-[disabled]:opacity-50
                            data-[disabled]:[&_[role=slider]]:bg-zinc-400
                            ${isEditing ? "cursor-pointer" : "cursor-default"}
                            `}
                        />

                        <div className="flex justify-between text-xs text-muted-foreground mt-0.5 select-none">
                            <span>{euroFormatter.format(MIN)}</span>
                            <span>{euroFormatter.format(MAX)}</span>
                        </div>
                    </div>
                </div>

                <div
                    className="flex flex-col gap-4
                    lg:flex-row lg:justify-between
                    border-t border-zinc-300 mt-4 pt-4"
                >
                    <div className="select-none">
                        {pendingLimit && (
                            <div>
                                <p className="text-xs font-medium text-yellow-500">
                                    {t("protection.limits.myLimits.error")}
                                </p>
                                <p className="text-xs font-normal">
                                    <span className="font-medium">{t("protection.limits.myLimits.lossPending")}{" "}</span>
                                    {euroFormatter.format(pendingLimit.value ?? 0)}{" "}{pendingLimit.period}{", "}
                                    {t("protection.limits.myLimits.applyFrom")} {pendingLimit.start &&
                                    new Date(new Date(pendingLimit.start).getTime() + 60 * 60 * 1000) // +1 hour because background service every 1 hour
                                        .toLocaleString("el-GR", {
                                            day: "2-digit",
                                            month: "2-digit",
                                            year: "numeric",
                                            hour: "2-digit",
                                            minute: "2-digit",
                                            hourCycle: "h23",
                                        })}
                                </p>
                            </div>
                        )}

                        {isEditing && (
                            <p className="text-xs font-normal">
                                <span className="font-medium">{t("protection.limits.myLimits.loss")}{" "}</span>
                                {euroFormatter.format(lossDailyLimit ?? 0)}{" "}{t("protection.limits.myLimits.day")}{", "}
                                {euroFormatter.format(lossWeeklyLimit ?? 0)}{" "}{t("protection.limits.myLimits.week")}{", "}
                                {euroFormatter.format(lossMonthlyLimit ?? 0)}{" "}{t("protection.limits.myLimits.month")}
                            </p>
                        )}

                        {isEditing && newLimitPreview() && (
                            <p className="text-xs font-normal mt-2">
                                {newLimitPreview()}
                            </p>
                        )}
                    </div>

                    {!isEditing && (
                        <div className="flex justify-end">
                            <Button
                                onClick={onEdit}
                                disabled={hasPending}
                                className="
                                px-6 bg-casinoapp-dark-blue/95 text-zinc-100
                                hover:bg-casinoapp-dark-blue hover:text-white cursor-pointer"
                            >
                                {t("protection.edit")}
                            </Button>
                        </div>
                    )}

                    {isEditing && (
                        <div className="flex justify-end gap-8">
                            <Button
                                onClick={onReset}
                                className="
                                px-8 bg-white border-2 border-casinoapp-light-blue
                                text-casinoapp-light-blue font-semibold hover:bg-zinc-50
                                hover:border-casinoapp-dark-blue hover:text-casinoapp-dark-blue cursor-pointer"
                            >
                                {t("protection.reset")}
                            </Button>

                            <Button
                                disabled={isSaveChangesDisabled}
                                onClick={onSave}
                                className="
                                px-8 bg-casinoapp-dark-blue/95 text-zinc-100
                                hover:bg-casinoapp-dark-blue hover:text-white cursor-pointer"
                            >
                                {t("protection.save")}
                            </Button>
                        </div>
                    )}
                </div>
            </CardContent>
        </Card>
    );
};

export default LossLimitCard;