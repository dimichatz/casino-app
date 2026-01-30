import {Controller, type UseFormReturn, useWatch} from "react-hook-form";
import { GENDER_VALUES} from "@/schemas/core/enums";
import type { RegisterFields } from "@/schemas/register";
import { useTranslation } from "react-i18next";
import {CircleX} from "lucide-react";
import {useEffect, useMemo} from "react";
import {debounce} from "@/utils/debounce.ts";
import {checkPhoneExists} from "@/services/api.auth-checks.ts";
import {Input} from "@/components/ui/input.tsx";
import {RadioGroup, RadioGroupItem} from "@/components/ui/radio-group.tsx";
import {CasinoBirthDateInput} from "@/components/auth/steps/BirthDateInput.tsx";

type Props = {
    form: UseFormReturn<RegisterFields>;
};

export const Step2Identity = ({ form }: Props) => {
    const { t } = useTranslation("auth");

    const {
        register,
        control,
        setError,
        clearErrors,
        formState: { errors, dirtyFields },
    } = form;

    const phoneNumberValue = useWatch({
        control: form.control,
        name: "phoneNumber",
    });

    const debouncedCheckPhoneNumber = useMemo(
        () =>
            debounce(async (value: string) => {
                if (!value || value.length != 10 ) return;
                const fullPhoneNumber = `+30${value}`;

                const exists = await checkPhoneExists(fullPhoneNumber);

                if (exists) {
                    setError("phoneNumber", {
                        type: "server",
                        message: "register.errors.phoneNumber.exists",
                    });
                } else {
                    clearErrors("phoneNumber");
                }
            }, 500),
        [setError, clearErrors]
    );

    useEffect(() => {
        if (!dirtyFields.phoneNumber) return;
        debouncedCheckPhoneNumber(phoneNumberValue);
    }, [phoneNumberValue, dirtyFields.phoneNumber, debouncedCheckPhoneNumber]);

    return (
        <div className="space-y-4">
            <div>
                <label
                    htmlFor="phoneNumber"
                    className="block text-sm font-medium text-casinoapp-black"
                >
                    {t("register.phoneNumber")}
                </label>

                <div className="flex gap-5">
                    <div className="w-15 rounded-md bg-casinoapp-light-gray/40 px-4 py-2 mt-0.5">
                        <p className="text-casinoapp-black">+30</p>
                    </div>

                    <Input
                        id="phoneNumber"
                        {...register("phoneNumber")}
                        autoComplete="tel"
                        placeholder={t("register.phoneNumberInput")}
                        className={`
                            w-70 mt-0.5
                            focus-visible:ring-0
                            focus-visible:ring-offset-0
                        ${
                            errors.phoneNumber
                                ? "border-red-500 focus-visible:border-red-500"
                                : "border-zinc-300 focus-visible:border-zinc-400"
                            }
                        `}
                    />
                </div>
                {errors.phoneNumber && (
                    <div className="flex items-center gap-1 mt-2 text-xs text-red-500">
                        <CircleX className="h-4 w-4" />
                        <span>
                            {t(errors.phoneNumber.message as string)}
                        </span>
                    </div>
                )}
            </div>

            <div>
                <label
                    htmlFor="firstname"
                    className="block text-sm font-medium text-casinoapp-black"
                >
                    {t("register.firstname")}
                </label>
                <Input
                    id="firstname"
                    {...register("firstname")}
                    autoComplete="given-name"
                    placeholder={t("register.firstnameInput")}
                    className={`
                            w-90 mt-0.5
                            focus-visible:ring-0
                            focus-visible:ring-offset-0
                        ${
                        errors.firstname
                            ? "border-red-500 focus-visible:border-red-500"
                            : "border-zinc-300 focus-visible:border-zinc-400"
                        }
                    `}
                />
                {errors.firstname && (
                    <div className="flex items-center gap-1 mt-2 text-xs text-red-500">
                        <CircleX className="h-4 w-4" />
                        <span>
                            {t(errors.firstname.message as string)}
                        </span>
                    </div>
                )}
            </div>

            <div>
                <label
                    htmlFor="lastname"
                    className="block text-sm font-medium text-casinoapp-black"
                >
                    {t("register.lastname")}
                </label>
                <Input
                    id="lastname"
                    {...register("lastname")}
                    autoComplete="family-name"
                    placeholder={t("register.lastnameInput")}
                    className={`
                            w-90 mt-0.5
                            focus-visible:ring-0
                            focus-visible:ring-offset-0
                        ${
                        errors.lastname
                            ? "border-red-500 focus-visible:border-red-500"
                            : "border-zinc-300 focus-visible:border-zinc-400"
                    }
                    `}
                />
                {errors.lastname && (
                    <div className="flex items-center gap-1 mt-2 text-xs text-red-500">
                        <CircleX className="h-4 w-4" />
                        <span>
                            {t(errors.lastname.message as string)}
                        </span>
                    </div>
                )}
            </div>

            <div>
                <fieldset>
                    <legend className="block text-sm font-medium text-casinoapp-black">
                        {t("register.gender")}
                    </legend>

                    <RadioGroup
                        value={form.watch("gender")}
                        onValueChange={(value) =>
                            form.setValue("gender", value as RegisterFields["gender"], {
                                shouldDirty: true,
                                shouldTouch: true,
                            })
                        }
                        className="flex gap-10 mt-0.5"
                    >
                        {GENDER_VALUES.map((g) => {
                            const id = `gender-${g}`;

                            return (
                                <div
                                    key={g}
                                    className="flex items-center w-40 h-10 rounded-md bg-white
                                        px-4 py-2 gap-2 cursor-pointer border
                                        border-zinc-300 hover:border-zinc-400"
                                >
                                    <RadioGroupItem
                                        id={id}
                                        value={g}
                                        className="border-zinc-400
                                            data-[state=checked]:border-casinoapp-dark-blue
                                            data-[state=checked]:text-casinoapp-dark-blue"
                                    />

                                    <label
                                        htmlFor={id}
                                        className="text-sm text-casinoapp-black cursor-pointer"
                                    >
                                        {g}
                                    </label>
                                </div>
                            );
                        })}
                    </RadioGroup>
                </fieldset>

            </div>

            <div>
                <label
                    htmlFor="birthDate"
                    className="block text-sm font-medium text-casinoapp-black mb-0.5"
                >
                    {t("register.birthDate")}
                </label>
                <Controller
                    name="birthDate"
                    control={control}
                    render={({ field }) => (
                        <CasinoBirthDateInput
                            id="birthDate"
                            value={field.value}
                            onChange={field.onChange}
                            error={!!errors.birthDate}
                        />
                    )}
                />
                {errors.birthDate && (
                    <div className="flex items-center gap-1 mt-2 text-xs text-red-500">
                        <CircleX className="h-4 w-4" />
                        <span>
                            {t(errors.birthDate.message as string)}
                        </span>
                    </div>
                )}
            </div>
        </div>
    );
};
