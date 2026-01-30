import {useForm, useWatch} from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Form } from "@/components/ui/form";
import { useTranslation } from "react-i18next";
import { loginSchema } from "@/schemas/login";
import type { UserLoginDTO } from "@/schemas/login";
import { useAuth } from "@/hooks/useAuth";
import {Button} from "@/components/ui/button.tsx";
import {FormControl, FormField, FormItem, FormLabel} from "@/components/ui/form.tsx";
import {Input} from "@/components/ui/input.tsx";
import {useEffect, useState} from "react";
import {CircleX, Eye, EyeOff} from "lucide-react";
import axios from "axios";
import {LOGIN_ERROR_MAP} from "@/errors/authErrors.ts";

type LoginFormProps = {
    onSuccess: () => void;
};

const LoginForm = ({ onSuccess }: LoginFormProps) => {
    const { t } = useTranslation("auth");
    const [password, setPassword] = useState(false);

    const form = useForm<UserLoginDTO>({
        resolver: zodResolver(loginSchema),
        defaultValues: {
            usernameOrEmail: "",
            password: "",
        },
    });

    const { loginUser } = useAuth();

    const usernameOrEmailValue = useWatch({
        control: form.control,
        name: "usernameOrEmail",
    });

    const passwordValue = useWatch({
        control: form.control,
        name: "password",
    });

    const isEmpty =
        !usernameOrEmailValue?.trim() ||
        !passwordValue?.trim();

    useEffect(() => {
        if (form.formState.errors.root) {
            form.clearErrors("root");
        }
    }, [usernameOrEmailValue, passwordValue, form]);

    const onSubmit = async (data: UserLoginDTO) => {
        try {
            await loginUser(data);
            onSuccess();
        } catch (err: unknown) {

            let backendCode: string | null = null;
            let message = "login.errors.generic";

            if (axios.isAxiosError(err)) {
                backendCode =
                    err.response?.data?.code ??
                    err.code ??
                    null;
            }

            if (backendCode && backendCode in LOGIN_ERROR_MAP) {
                message = LOGIN_ERROR_MAP[backendCode];
            }

            form.setError("root", {
                type: "server",
                message,
            });
        }
    };

    return (
        <Form {...form}>
        <form
            onSubmit={form.handleSubmit(onSubmit)}
            className="space-y-5"
        >
            <FormField
                control={form.control}
                name="usernameOrEmail"
                render={({ field }) => {
                    const hasError = !!form.formState.errors.root;

                    return(
                        <FormItem>
                            <FormLabel>
                                {t("login.username")}
                            </FormLabel>
                            <FormControl>
                                <Input
                                    {...field}
                                    type="text"
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
                        </FormItem>
                    );
                }}
            />

            {/* PASSWORD */}
            <FormField
                control={form.control}
                name="password"
                render={({ field }) => {
                    const hasValue = !!field.value;
                    const hasError = !!form.formState.errors.root;

                    return(
                        <FormItem>
                            <FormLabel>
                                {t("login.password")}
                            </FormLabel>

                            <div className="relative">
                                <FormControl>
                                    <Input
                                        {...field}
                                        type={password ? "text" : "password"}
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
                                        onClick={() => setPassword(v => !v)}
                                        className="absolute cursor-pointer right-3 top-1/2 -translate-y-1/2
                                            text-zinc-500"
                                        aria-label={password ? "Hide password" : "Show password"}
                                    >
                                        {password ? <EyeOff size={18} /> : <Eye size={18} />}
                                    </button>
                                )}
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

            <Button
                type="submit"
                disabled={
                    form.formState.isSubmitting || !!form.formState.errors.root ||
                    isEmpty
                }
                className="w-80 rounded-md mt-4 text-casinoapp-black
                  bg-casinoapp-orange py-2 font-semibold cursor-pointer
                  hover:bg-casinoapp-orange/90 disabled:opacity-50 disabled:cursor-default"
            >
                {t("login.button")}
            </Button>
        </form>
        </Form>
    );
};

export default LoginForm;
