import {Form, FormControl, FormField, FormItem, FormLabel} from "@/components/ui/form.tsx";
import {Input} from "@/components/ui/input.tsx";
import {useForm, useWatch} from "react-hook-form";
import {Button} from "@/components/ui/button.tsx";
import {useTranslation} from "react-i18next";
import {type UserChangePasswordDTO, passwordSchema} from "@/schemas/account/profile/protection.ts";
import {zodResolver} from "@hookform/resolvers/zod";
import {CheckCircle, Circle, CircleX, Eye, EyeOff} from "lucide-react";
import {useEffect, useState} from "react";
import { changePlayerPassword  } from "@/services/api.player.ts";
import axios from "axios";
import {toast} from "sonner";
import {PASSWORD_ERROR_MAP} from "@/errors/profileErrors.ts";

type RequirementStatus = "empty" | "valid" | "invalid";

type RequirementProps = {
    status: RequirementStatus;
    label: string;
};

const passwordRules = {
    length: (v: string) => v.length >= 8 && v.length <= 15,
    upper: (v: string) => /[A-Z]/.test(v),
    lower: (v: string) => /[a-z]/.test(v),
    digit: (v: string) => /\d/.test(v),
    special: (v: string) => /\W/.test(v),
};

const PasswordRequirement = ({ status, label }: RequirementProps) => {
    const icon =
        status === "valid" ? (
            <CheckCircle className="h-4 w-4 text-green-500" />
        ) : status === "invalid" ? (
            <CircleX className="h-4 w-4 text-red-500" />
        ) : (
            <Circle className="h-4 w-4 text-zinc-400" />
        );

    return (
        <div className="flex items-center gap-2 text-xs">
            {icon}
            <span className="text-zinc-400">{label}</span>
        </div>
    );
};

const ChangePasswordForm = () => {
    const { t } = useTranslation("profile");

    const [currentPassword, setCurrentPassword] = useState(false);
    const [newPassword, setNewPassword] = useState(false);

    const form = useForm<UserChangePasswordDTO>({
        resolver: zodResolver(passwordSchema),
        defaultValues: {
            currentPassword: "",
            newPassword: "",
        },
        mode: "onChange",
    });

    const currentPasswordValue = useWatch({
        control: form.control,
        name: "currentPassword",
    });

    const newPasswordValue = useWatch({
        control: form.control,
        name: "newPassword",
    });

    useEffect(() => {
        if (form.formState.errors.root) {
            form.clearErrors("root");
        }
    }, [currentPasswordValue, newPasswordValue, form]);

    const onSubmit = async (data: UserChangePasswordDTO) => {
        try {
            await changePlayerPassword(data);
            toast.success(t("security.success"));
        } catch (err: unknown) {
            console.warn("Request rejected:", err);

            let backendCode: string | null = null;
            let message = "security.errors.generic";

            if (axios.isAxiosError(err)) {
                backendCode =
                    err.response?.data?.code ??
                    err.code ??
                    null;
            }

            if (backendCode && backendCode in PASSWORD_ERROR_MAP) {
                message = PASSWORD_ERROR_MAP[backendCode];
            }

            form.setError("root", {
                type: "server",
                message,
            });
        }
    };

    return(
        <Form {...form}>
            <form
                onSubmit={form.handleSubmit(onSubmit)}
                className="space-y-4"
            >
                <FormField
                    control={form.control}
                    name="currentPassword"
                    render={({ field }) => {
                        const hasValue = !!field.value;
                        const hasError = !!form.formState.errors.root;

                        return(
                        <FormItem >
                            <FormLabel className="text-casinoapp-black">
                                {t("security.currentPassword")}
                            </FormLabel>

                            <div className="relative">
                                <FormControl>
                                    <Input
                                        type={currentPassword ? "text" : "password"}
                                        {...field}
                                        className={`
                                        focus-visible:ring-0
                                        focus-visible:ring-offset-0
                                    ${
                                            hasError
                                                ? "border-red-500  focus-visible:border-red-500"
                                                : "border-zinc-300 focus-visible:border-zinc-400"
                                        }
                                    `}
                                    />
                                </FormControl>
                                {hasValue && (
                                    <button
                                        type="button"
                                        tabIndex={-1}
                                        onClick={() => setCurrentPassword(v => !v)}
                                        className="absolute cursor-pointer right-3 top-1/2 -translate-y-1/2
                                            text-zinc-500"
                                        aria-label={currentPassword ? "Hide password" : "Show password"}
                                    >
                                        {currentPassword ? <EyeOff size={18} /> : <Eye size={18} />}
                                    </button>
                                )}
                            </div>
                        </FormItem>
                    );
                    }}
                />

                <FormField
                    control={form.control}
                    name="newPassword"
                    render={({ field }) => {
                        const hasError = !!form.formState.errors.root;
                        const value = field.value ?? "";
                        const isEmpty = value.length === 0;
                        const hasValue = !!field.value;

                        const requirements = [
                            passwordRules.length,
                            passwordRules.upper,
                            passwordRules.lower,
                            passwordRules.digit,
                            passwordRules.special,
                        ];

                        const hasInvalidRequirement =
                            !isEmpty && requirements.some((rule) => !rule(value));

                        const ruleStatus = (
                            rule: (v: string) => boolean
                        ): RequirementStatus => {
                            if (isEmpty) return "empty";
                            return rule(value) ? "valid" : "invalid";
                        };

                        return (
                            <FormItem>
                                <FormLabel className="text-casinoapp-black">
                                    {t("security.newPassword")}
                                </FormLabel>

                                <div className="relative">
                                    <FormControl>
                                        <Input
                                            type={newPassword ? "text" : "password"}
                                            {...field}
                                            className={`
                                                focus-visible:ring-0
                                                focus-visible:ring-offset-0
                                                ${
                                                hasInvalidRequirement || hasError
                                                    ? "border-red-500 focus-visible:border-red-500"
                                                    : "border-zinc-300 focus-visible:border-zinc-400"
                                                }
                                            `}
                                        />
                                    </FormControl>
                                    {hasValue && (
                                        <button
                                            tabIndex={-1}
                                            type="button"
                                            onClick={() => setNewPassword(v => !v)}
                                            className="absolute cursor-pointer right-3 top-1/2 -translate-y-1/2
                                                text-zinc-500"
                                            aria-label={newPassword ? "Hide password" : "Show password"}
                                        >
                                            {newPassword ? <EyeOff size={18} /> : <Eye size={18} />}
                                        </button>
                                    )}
                                </div>

                                <div className="mt-2 space-y-1 text-sm text-muted-foreground">
                                    <PasswordRequirement
                                        status={ruleStatus(passwordRules.length)}
                                        label={t("security.passwordRules.length")}
                                    />
                                    <PasswordRequirement
                                        status={ruleStatus(passwordRules.upper)}
                                        label={t("security.passwordRules.upper")}
                                    />
                                    <PasswordRequirement
                                        status={ruleStatus(passwordRules.lower)}
                                        label={t("security.passwordRules.lower")}
                                    />
                                    <PasswordRequirement
                                        status={ruleStatus(passwordRules.digit)}
                                        label={t("security.passwordRules.digit")}
                                    />
                                    <PasswordRequirement
                                        status={ruleStatus(passwordRules.special)}
                                        label={t("security.passwordRules.special")}
                                    />
                                </div>
                            </FormItem>
                        );
                    }}
                />

                {form.formState.errors.root && (
                    <div className="flex items-center gap-1 mt-2 text-xs text-red-500">
                        <CircleX className="h-4 w-4" />
                        <span>
                            {t(form.formState.errors.root.message as string)}
                        </span>
                    </div>
                )}

                <div className="flex justify-end mt-6">
                    <Button
                        className="text-zinc-200 bg-casinoapp-black/95
                        hover:bg-casinoapp-black hover:text-white cursor-pointer"
                        type="submit"
                        disabled={!form.formState.isValid || form.formState.isSubmitting}
                    >
                        {t("security.save")}
                    </Button>
                </div>
            </form>
        </Form>
    );
};

export default ChangePasswordForm;