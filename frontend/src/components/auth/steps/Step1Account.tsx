import type { UseFormReturn } from "react-hook-form";
import type { RegisterFields } from "@/schemas/register";
import { useTranslation } from "react-i18next";
import {CheckCircle, Circle, CircleX, Eye, EyeOff} from 'lucide-react';
import {useState, useEffect} from "react";
import { useMemo } from "react";
import { debounce } from "@/utils/debounce";
import { checkEmailExists, checkUsernameExists } from "@/services/api.auth-checks.ts";
import { useWatch } from "react-hook-form";
import {Input} from "@/components/ui/input.tsx";

type Props = {
    form: UseFormReturn<RegisterFields>;
};

type RequirementStatus = "empty" | "valid" | "invalid";

type RequirementProps = {
    status: RequirementStatus;
    label: string;
};

const passwordRules = {
    length: (v: string) => v.length >= 8 && v.length <= 50,
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

export const Step1Account = ({ form }: Props) => {
    const { t } = useTranslation("auth");

    const [showPassword, setShowPassword] = useState(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);
    const {
        register,
        setError,
        clearErrors,
        formState: { errors, dirtyFields },
    } = form;

    const emailValue = useWatch({
        control: form.control,
        name: "email",
    });

    const usernameValue = useWatch({
        control: form.control,
        name: "username",
    });

    const passwordValue = useWatch({
        control: form.control,
        name: "password",
    }) ?? "";

    const confirmPasswordValue = useWatch({
        control: form.control,
        name: "confirmPassword",
    }) ?? "";

    const debouncedCheckUsername = useMemo(
        () =>
            debounce(async (value: string) => {
                if (!value || value.length < 4) return;

                const exists = await checkUsernameExists(value);

                if (exists) {
                    setError("username", {
                        type: "server",
                        message: "register.errors.username.exists",
                    });
                } else {
                    clearErrors("username");
                }
            },),
        [setError, clearErrors]
    );

    const debouncedCheckEmail = useMemo(
        () =>
            debounce(async (value: string) => {
                if (!value) return;
                if (!value.includes("@")) return;

                const exists = await checkEmailExists(value);

                if (exists) {
                    setError("email", {
                        type: "server",
                        message: "register.errors.email.exists",
                    });
                } else {
                    clearErrors("email");
                }
            }, 500),
        [setError, clearErrors]
    );

    useEffect(() => {
        if (!dirtyFields.username) return;
        debouncedCheckUsername(usernameValue);
    }, [usernameValue, dirtyFields.username, debouncedCheckUsername]);

    useEffect(() => {
        if (!dirtyFields.email) return;
        debouncedCheckEmail(emailValue);
    }, [emailValue, dirtyFields.email, debouncedCheckEmail]);


    const isPasswordEmpty = passwordValue.length === 0;
    const hasPasswordValue = passwordValue.length > 0;
    const hasConfirmPasswordValue = confirmPasswordValue.length > 0;

    const requirements = [
        passwordRules.length,
        passwordRules.upper,
        passwordRules.lower,
        passwordRules.digit,
        passwordRules.special,
    ];

    const hasInvalidRequirement =
        !isPasswordEmpty && requirements.some((rule) => !rule(passwordValue));

    const ruleStatus = (rule: (v: string) => boolean): RequirementStatus => {
        if (isPasswordEmpty) return "empty";
        return rule(passwordValue) ? "valid" : "invalid";
    };

    return (
        <div className="space-y-4">

            <div>
                <label
                    htmlFor="email"
                    className="block text-sm font-medium text-casinoapp-black"
                >
                    {t("register.email")}
                </label>
                <Input
                    id="email"
                    {...register("email")}
                    autoComplete="email"
                    placeholder={t("register.emailInput")}
                    className={`
                            w-90 mt-0.5
                            focus-visible:ring-0
                            focus-visible:ring-offset-0
                        ${
                        errors.email
                            ? "border-red-500 focus-visible:border-red-500"
                            : "border-zinc-300 focus-visible:border-zinc-400"
                        }
                    `}
                />
                {errors.email && (
                    <div className="flex items-center gap-1 mt-2 text-xs text-red-500">
                        <CircleX className="h-4 w-4" />
                        <span>
                            {t(errors.email.message as string)}
                        </span>
                    </div>

                )}
            </div>

            <div>
                <label
                    htmlFor="username"
                    className="block text-sm font-medium text-casinoapp-black"
                >
                    {t("register.username")}
                </label>
                <Input
                    id="username"
                    {...register("username")}
                    autoComplete="username"
                    placeholder={t("register.usernameInput")}
                    className={`
                            w-90 mt-0.5
                            focus-visible:ring-0
                            focus-visible:ring-offset-0
                        ${
                        errors.username
                            ? "border-red-500 focus-visible:border-red-500"
                            : "border-zinc-300 focus-visible:border-zinc-400"
                    }
                    `}
                />
                {errors.username && (
                    <div className="flex items-center gap-1 mt-2 text-xs text-red-500">
                        <CircleX className="h-4 w-4" />
                        <span>
                            {t(errors.username.message as string)}
                        </span>
                    </div>
                )}
            </div>

            <div>
                <label
                    htmlFor="password"
                    className="block text-sm font-medium text-casinoapp-black"
                >
                    {t("register.password")}
                </label>

                <div className="relative">
                    <Input
                        id="password"
                        type={showPassword ? "text" : "password"}
                        {...register("password")}
                        autoComplete="new-password"
                        placeholder=""
                        className={`
                                w-90 mt-0.5
                                focus-visible:ring-0
                                focus-visible:ring-offset-0
                            ${
                            errors.password || hasInvalidRequirement
                                ? "border-red-500 focus-visible:border-red-500"
                                : "border-zinc-300 focus-visible:border-zinc-400"
                            }
                        `}
                    />
                    {hasPasswordValue && (
                        <button
                            type="button"
                            tabIndex={-1}
                            onClick={() => setShowPassword((v) => !v)}
                            className="absolute right-3 top-1/2 -translate-y-1/2
                                text-zinc-500 hover:text-zinc-700 cursor-pointer"
                            aria-label={showPassword ? "Hide password" : "Show password"}
                        >
                            {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
                        </button>
                    )}
                </div>

                {errors.password && (
                    <div className="flex items-center gap-1 mt-2 text-xs text-red-500">
                        <CircleX className="h-4 w-4" />
                        <span className="max-w-85">
                            {t(errors.password.message as string)}
                        </span>
                    </div>
                )}

                <div className="mt-2 space-y-1">
                    <PasswordRequirement
                        status={ruleStatus(passwordRules.length)}
                        label={t("profile:security.passwordRules.length")}
                    />
                    <PasswordRequirement
                        status={ruleStatus(passwordRules.upper)}
                        label={t("profile:security.passwordRules.upper")}
                    />
                    <PasswordRequirement
                        status={ruleStatus(passwordRules.lower)}
                        label={t("profile:security.passwordRules.lower")}
                    />
                    <PasswordRequirement
                        status={ruleStatus(passwordRules.digit)}
                        label={t("profile:security.passwordRules.digit")}
                    />
                    <PasswordRequirement
                        status={ruleStatus(passwordRules.special)}
                        label={t("profile:security.passwordRules.special")}
                    />
                </div>
            </div>

            <div>
                <label
                    htmlFor="confirmPassword"
                    className="block text-sm font-medium text-casinoapp-black"
                >
                    {t("register.confirmPassword")}
                </label>

                <div className="relative">
                    <Input
                        id="confirmPassword"
                        type={showConfirmPassword ? "text" : "password"}
                        {...register("confirmPassword")}
                        autoComplete="new-password"
                        placeholder=""
                        className={`
                                w-90 mt-0.5
                                focus-visible:ring-0
                                focus-visible:ring-offset-0
                            ${
                            errors.confirmPassword
                                ? "border-red-500 focus-visible:border-red-500"
                                : "border-zinc-300 focus-visible:border-zinc-400"
                        }
                        `}
                    />
                    {hasConfirmPasswordValue && (
                        <button
                            type="button"
                            tabIndex={-1}
                            onClick={() => setShowConfirmPassword((v) => !v)}
                            className="absolute right-3 top-1/2 -translate-y-1/2
                                text-zinc-500 hover:text-zinc-700 cursor-pointer"
                            aria-label={showConfirmPassword ? "Hide password" : "Show password"}
                        >
                            {showConfirmPassword ? <EyeOff size={18} /> : <Eye size={18} />}
                        </button>
                    )}
                </div>

                {errors.confirmPassword && (
                    <div className="flex items-center gap-1 mt-2 text-xs text-red-500">
                        <CircleX className="h-4 w-4" />
                        <span>
                            {t(errors.confirmPassword.message as string)}
                        </span>
                    </div>
                )}
            </div>
        </div>
    );
};