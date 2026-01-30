import EditableRow from "@/components/account/profile/player-details/EditableRow.tsx";
import {useTranslation} from "react-i18next";
import {useState, useEffect, useMemo} from "react";
import {Input} from "@/components/ui/input.tsx";
import {Form, FormControl, FormField, FormItem} from "@/components/ui/form.tsx";
import {Button} from "@/components/ui/button.tsx";
import {useForm, useWatch} from "react-hook-form";
import {emailSchema, type UserUpdateEmailDTO} from "@/schemas/account/profile/details.ts";
import {zodResolver} from "@hookform/resolvers/zod";
import axios from "axios";
import {toast} from "sonner";
import {updatePlayer} from "@/services/api.player.ts";
import {DETAILS_ERROR_MAP} from "@/errors/profileErrors.ts";
import {CircleX} from "lucide-react";
import {checkEmailExists} from "@/services/api.auth-checks.ts";
import { useDebouncedValue } from "@/hooks/useDebouncedValue";
import {z} from "zod";

interface EditEmailCardProps {
    email?: string;
    onUpdated?: () => Promise<void>;
    open: boolean;
    disabled: boolean;
    onToggle: () => void;
}

type EmailFormValues = z.infer<typeof emailSchema>;

const EditEmailCard = ({
                           email,
                           onUpdated,
                           open,
                           disabled,
                           onToggle,
                       }: EditEmailCardProps) => {
    const { t } = useTranslation("profile");
    const [isChecking, setIsChecking] = useState(false);

        const form = useForm<EmailFormValues>({
            resolver: zodResolver(emailSchema),
            defaultValues: {
                email: email ?? "",
            },
            mode: "onSubmit",
            reValidateMode: "onSubmit",
        });

    const defaultValues = useMemo(
        () => ({
            email: email ?? "",
        }),
        [email]
    );

    useEffect(() => {
        if (open) {
            form.reset(defaultValues);
        }
    }, [open, defaultValues, form]);

    const rawEmail = useWatch({
        control: form.control,
        name: "email",
    }) || "";
    const debouncedEmail = useDebouncedValue(rawEmail, 800);

    const trimmedRaw = rawEmail.trim();
    const trimmedDebounced = debouncedEmail.trim();
    const originalEmail = (email ?? "").trim();

    const isDisabled =
        disabled ||
        isChecking ||
        form.formState.isSubmitting ||
        trimmedRaw === "" ||
        trimmedRaw === originalEmail ||
        !!form.formState.errors.email ||
        !!form.formState.errors.root;

    const errorMessage =
        trimmedRaw === trimmedDebounced
            ? form.formState.errors.email?.message ||
            form.formState.errors.root?.message
            : undefined;

    useEffect(() => {
        const runValidation = async () => {
            // User is still typing
            if (trimmedRaw !== trimmedDebounced) {
                setIsChecking(true);
                form.clearErrors(["email"]);
                return;
            }

            if (!trimmedDebounced || trimmedDebounced === originalEmail) {
                setIsChecking(false);
                form.clearErrors(["email"]);
                return;
            }

            setIsChecking(true);

            // Run zod validation after debounce
            const schemaValid = await form.trigger("email");
            if (!schemaValid)
            {
                setIsChecking(false);
                return;
            }

            // Run backend check
            try {
                const exists = await checkEmailExists(trimmedDebounced);
                if (exists) {
                    form.setError("root", {
                        type: "server",
                        message: "details.errors.userEmailAlreadyExists",
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
    }, [trimmedDebounced, trimmedRaw, originalEmail, form]);

    const onSubmit = async (data: EmailFormValues) => {
        form.clearErrors("root");

        try {
            const dto: UserUpdateEmailDTO = {
                email: data.email,
            };

            await updatePlayer({
                userDetails: dto,
            });
            await onUpdated?.();

            toast.success(t("details.email.success"));
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
            label={t("details.email.title")}
            value={email}
            helperText={t("details.email.helper")}
            open={open}
            onToggle={onToggle}
            disabled={disabled}
            rounded="top"
        >
            <div className="flex items-center">
                <Form {...form}>
                    <form
                        onSubmit={form.handleSubmit(onSubmit)}
                    >
                        <FormField
                            control={form.control}
                            name="email"
                            render={({ field }) => {
                                const hasError =
                                    !!form.formState.errors.email || !!form.formState.errors.root;

                                return (
                                    <FormItem>
                                        <FormControl>
                                            <Input
                                                {...field}
                                                type="email"
                                                autoComplete="email"
                                                placeholder={t("auth:register.emailInput")}
                                                onChange={(e) => {
                                                    field.onChange(e.target.value);
                                                    setIsChecking(true);
                                                    form.clearErrors("root");
                                                }}
                                                className={`
                                                        focus-visible:ring-0
                                                        focus-visible:ring-offset-0
                                                    ${
                                                    hasError
                                                        ? "border-red-500 focus-visible:border-red-500"
                                                        : "border-zinc-300 focus-visible:border-zinc-400"
                                                    }
                                                `}
                                            />
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

export default EditEmailCard;