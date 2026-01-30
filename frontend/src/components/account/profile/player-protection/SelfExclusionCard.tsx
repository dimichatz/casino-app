import CollapsibleCard from "@/components/account/profile/player-protection/CollapsibleCard.tsx";
import {useTranslation} from "react-i18next";
import { Checkbox } from "@/components/ui/checkbox";
import {
    AlertDialog, AlertDialogAction, AlertDialogCancel,
    AlertDialogContent, AlertDialogDescription, AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle,
    AlertDialogTrigger
} from "@/components/ui/alert-dialog";
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select";
import {SELF_EXCLUSION_PERIOD_VALUES, type SelfExclusionPeriod} from "@/schemas/core/enums.ts";
import {useState} from "react";
import {SELF_EXCLUSION_PERIOD_I18N_KEY} from "@/i18n/enumMappings.ts";
import {Button} from "@/components/ui/button.tsx";
import {updatePlayer} from "@/services/api.player.ts";
import {toast} from "sonner";
import axios from "axios";
import {CircleX} from "lucide-react";
import {EXCLUSION_ERROR_MAP} from "@/errors/profileErrors.ts";

const SelfExclusionCard = () => {
    const { t } = useTranslation("profile");

    const [enabled, setEnabled] = useState(false);
    const [period, setPeriod] = useState<SelfExclusionPeriod | null>(null);

    const isSaveDisabled = enabled && period === null;
    const [error, setError] = useState<string | null>(null);

    const handleSave = async () => {
        if (!enabled || period === null) return;

        try {
            await updatePlayer({
                selfExclusionDetails: {
                    selfExclusionPeriod: period,
                },
            });

            toast.success(t("protection.exclusion.success"));
        } catch (err: unknown) {
            console.warn("Request rejected:", err);

            let backendCode: string | null = null;
            let key = "protection.exclusion.errors.generic";

            if (axios.isAxiosError(err)) {
                backendCode =
                    err.response?.data?.code ??
                    err.code ??
                    null;
            }

            if (backendCode && backendCode in EXCLUSION_ERROR_MAP) {
                key = EXCLUSION_ERROR_MAP[backendCode];
            }
            setError(key);
        }
    };

    return (
        <CollapsibleCard
            title={t("protection.exclusion.title")}
            rounded="bottom"
        >
            <div className="flex flex-col gap-6 select-none">
                <div
                    className="py-4 flex flex-col text-sm gap-4
                    pointer-events-none"
                >
                    <p>
                        {t("protection.exclusion.description")}
                    </p>
                    <p>
                        {t("protection.exclusion.sub-description-1")}
                    </p>
                    <p>
                        {t("protection.exclusion.sub-description-2")}
                    </p>
                </div>

                <div className="flex gap-3">
                    <Checkbox
                        className="h-6 w-6 border-zinc-400
                    data-[state=checked]:bg-casinoapp-dark-blue
                    data-[state=checked]:text-white cursor-pointer"
                        id="enableSelfExclusion"
                        checked={enabled}
                        onCheckedChange={(checked) => setEnabled(!!checked)}
                    />
                    <label
                        htmlFor="enableSelfExclusion"
                        className="font-bold cursor-pointer"
                    >
                        {t("protection.exclusion.check")}
                    </label>
                </div>

                {enabled && (
                    <div className="mt-6 w-full flex flex-col gap-4">
                        <label className="block text-sm font-medium mb-2">
                            {t("protection.exclusion.periodLabel")}
                        </label>

                        <Select
                            value={period !== null ? String(period) : ""}
                            onValueChange={(value) => {
                                setPeriod(Number(value) as SelfExclusionPeriod);
                                setError(null);
                            }}
                        >
                            <SelectTrigger
                                id="selfExclusionPeriod"
                                className="
                            outline-none
                            ring-0
                            ring-offset-0
                            focus:ring-0
                            border-border
                            focus:border-transparent
                            data-[state=open]:ring-0
                            data-[state=open]:outline-none
                            data-[state=open]:border-zinc-400
                            cursor-pointer"
                            >
                                <SelectValue
                                    placeholder={t("protection.exclusion.select")}
                                />
                            </SelectTrigger>

                            <SelectContent>
                                {SELF_EXCLUSION_PERIOD_VALUES.map((value) => (
                                    <SelectItem key={value} value={String(value)}>
                                        {t(SELF_EXCLUSION_PERIOD_I18N_KEY[value])}
                                    </SelectItem>
                                ))}
                            </SelectContent>
                        </Select>

                        {error && (
                            <div className="flex items-center gap-1 mt-2 text-xs text-red-500">
                                <CircleX className="h-4 w-4" />
                                <span>
                                    {t(error)}
                                </span>
                            </div>
                        )}

                        <AlertDialog>
                            <AlertDialogTrigger asChild>
                                <Button
                                    type="button"
                                    disabled={isSaveDisabled}
                                    className="w-full text-zinc-200 bg-casinoapp-black/95
                                    hover:bg-casinoapp-black hover:text-white cursor-pointer"
                                >
                                    {t("protection.save")}
                                </Button>
                            </AlertDialogTrigger>

                            <AlertDialogContent>
                                <AlertDialogHeader>
                                    <AlertDialogTitle>
                                        {t("protection.exclusion.confirmTitle")}
                                    </AlertDialogTitle>

                                    <AlertDialogDescription>
                                        {t("protection.exclusion.confirmDescription")}
                                    </AlertDialogDescription>
                                </AlertDialogHeader>

                                <AlertDialogFooter>
                                    <AlertDialogCancel>
                                        {t("protection.exclusion.cancel")}
                                    </AlertDialogCancel>

                                    <AlertDialogAction
                                        onClick={handleSave}
                                        className="bg-destructive text-destructive-foreground hover:bg-destructive/80"
                                    >
                                        {t("protection.exclusion.confirmAction")}
                                    </AlertDialogAction>
                                </AlertDialogFooter>
                            </AlertDialogContent>
                        </AlertDialog>

                    </div>
                )}
            </div>
        </CollapsibleCard>
    );
};

export default SelfExclusionCard;