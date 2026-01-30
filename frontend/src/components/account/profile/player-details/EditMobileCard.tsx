import {useTranslation} from "react-i18next";
import EditableRow from "@/components/account/profile/player-details/EditableRow.tsx";
import {useEffect, useMemo, useState} from "react";
import {Form, FormControl, FormField, FormItem} from "@/components/ui/form.tsx";
import {Input} from "@/components/ui/input.tsx";
import {CircleX} from "lucide-react";
import {Button} from "@/components/ui/button.tsx";
import {useForm, useWatch} from "react-hook-form";
import {zodResolver} from "@hookform/resolvers/zod";
import {mobileSchema, type UserUpdateMobileDTO} from "@/schemas/account/profile/details.ts";
import {useDebouncedValue} from "@/hooks/useDebouncedValue.ts";
import {checkPhoneExists} from "@/services/api.auth-checks.ts";
import {updatePlayer} from "@/services/api.player.ts";
import {toast} from "sonner";
import axios from "axios";
import {DETAILS_ERROR_MAP} from "@/errors/profileErrors.ts";
import {z} from "zod";

interface EditMobileCardProps {
    phoneNumber?: string;
    onUpdated?: () => Promise<void>;
    open: boolean;
    disabled: boolean;
    onToggle: () => void;
}

type MobileFormValues = z.infer<typeof mobileSchema>;

const EditMobileCard = ({
                            phoneNumber,
                            onUpdated,
                            open,
                            disabled,
                            onToggle,
                        }: EditMobileCardProps) => {
    const { t } = useTranslation("profile");
    const [isChecking, setIsChecking] = useState(false);

    const originalLocalPhone =
        phoneNumber?.startsWith("+30") ? phoneNumber.slice(3) : phoneNumber ?? "";

    const form = useForm<MobileFormValues>({
        resolver: zodResolver(mobileSchema),
        defaultValues: {
            phoneNumber: originalLocalPhone ?? "", // 69
        },
        mode: "onSubmit",
        reValidateMode: "onSubmit",
    });

    const defaultValues = useMemo(
        () => ({
            phoneNumber: originalLocalPhone ?? "",
        }),
        [originalLocalPhone]
    );

    useEffect(() => {
        if (open) {
            form.reset(defaultValues);
        }
    }, [open, defaultValues, form]);

    const rawMobile = useWatch({
        control: form.control,
        name: "phoneNumber",
    }) || "";
    const debouncedPhoneNumber = useDebouncedValue(rawMobile, 800);

    const trimmedRaw = rawMobile.trim();
    const trimmedDebounced = debouncedPhoneNumber.trim(); // 69
    const originalMobile = (phoneNumber ?? "").trim();    // +3069
    const debouncedWithPrefix = `+30${trimmedDebounced}`;        // +3069
    const rawWithPrefix = `+30${trimmedRaw}`;                    // +3069

    const isDisabled =
        disabled ||
        isChecking ||
        form.formState.isSubmitting ||
        trimmedRaw === "" ||
        rawWithPrefix  === originalMobile ||
        !!form.formState.errors.phoneNumber ||
        !!form.formState.errors.root;

    const errorMessage =
        form.formState.errors.phoneNumber?.message ||
        form.formState.errors.root?.message;

    useEffect(() => {
        const runValidation = async () => {
            // User is still typing
            if (trimmedRaw !== trimmedDebounced) {
                setIsChecking(true);
                form.clearErrors(["phoneNumber"]);
                return;
            }


            if (!trimmedDebounced || debouncedWithPrefix === originalMobile) {
                setIsChecking(false);
                form.clearErrors(["phoneNumber"]);
                return;
            }

            setIsChecking(true);

            // Run zod validation after debounce
            const schemaValid = await form.trigger("phoneNumber");
            if (!schemaValid)
            {
                setIsChecking(false);
                return;
            }

            // Run backend check
            try {
                const exists = await checkPhoneExists(`+30${trimmedDebounced}`);
                if (exists) {
                    form.setError("root", {
                        type: "server",
                        message: "details.errors.userPhoneNumberAlreadyExists",
                    });
                    setIsChecking(false);
                } else {
                    form.clearErrors("root");
                    setIsChecking(false);
                }
            } catch {
                setIsChecking(false);
            }
        };

        void runValidation();
    }, [trimmedDebounced, trimmedRaw,debouncedWithPrefix, originalMobile, form]);

    const onSubmit = async (data: MobileFormValues) => {
        form.clearErrors("root");

        try {
            const dto: UserUpdateMobileDTO = {
                phoneNumber: `+30${data.phoneNumber}`,
            };

            await updatePlayer({
                userDetails: dto,
            });
            await onUpdated?.();

            toast.success(t("details.mobile.success"));
            onToggle();
        } catch (err: unknown) {
            console.warn("Request rejected:", err);

            let backendCode: string | null = null;
            let message = "details.errors.generic";

            if (axios.isAxiosError(err)) {
                backendCode =
                    err.response?.data?.code ??
                    err.code ??
                    null;
            }

            if (backendCode && backendCode in DETAILS_ERROR_MAP) {
                message = DETAILS_ERROR_MAP[backendCode];
            }

            form.setError("root", {
                type: "server",
                message,
            });
        }
    };

    return (
        <EditableRow
            label={t("details.mobile.title")}
            value={phoneNumber}
            helperText={t("details.mobile.helper")}
            open={open}
            disabled={disabled}
            onToggle={onToggle}
            rounded="none"
        >
            <div className="flex items-center">
                <Form {...form}>
                    <form
                        onSubmit={form.handleSubmit(onSubmit)}
                    >
                        <FormField
                            control={form.control}
                            name="phoneNumber"
                            render={({ field }) => {
                                const hasError =
                                    !!form.formState.errors.phoneNumber || !!form.formState.errors.root;

                                return (
                                    <FormItem>
                                        <FormControl>
                                            <div className="relative">
                                                <div
                                                    className="absolute inset-y-0 left-3 flex items-center gap-1 pr-2
                                                 border-r "
                                                >
                                                    <span className="fi fi-gr fis rounded-full text-xl" />

                                                    <span className="text-sm text-muted-foreground">
                                                    {"+30"}</span>
                                                </div>
                                                <Input
                                                    {...field}
                                                    type="tel"
                                                    autoComplete="tel"
                                                    placeholder={t("auth:register.phoneNumberInput")}
                                                    onChange={(e) => {
                                                        field.onChange(e.target.value);
                                                        setIsChecking(true);
                                                        form.clearErrors("root");
                                                    }}
                                                    className={`
                                                        pl-20
                                                        focus-visible:ring-0
                                                        focus-visible:ring-offset-0
                                                    ${
                                                        hasError
                                                            ? "border-red-500 focus-visible:border-red-500"
                                                            : "border-zinc-300 focus-visible:border-zinc-400"
                                                    }
                                                `}
                                                />
                                            </div>

                                        </FormControl>
                                    </FormItem>
                                );
                            }}
                        />
                        {errorMessage && (
                            <div className="flex items-center gap-1 mt-2 text-xs text-red-500">
                                <CircleX className="h-4 w-4" />
                                <span>
                            {t(errorMessage)}
                        </span>
                            </div>
                        )}

                        <Button
                            className="w-80 mt-6 text-zinc-200 bg-casinoapp-black/95
                        hover:bg-casinoapp-black hover:text-white cursor-pointer"
                            type="submit"
                            disabled={isDisabled}
                        >
                            {t("details.save")}
                        </Button>
                    </form>
                </Form>
            </div>
        </EditableRow>
    );
};

export default EditMobileCard;