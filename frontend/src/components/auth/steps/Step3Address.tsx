import {type UseFormReturn} from "react-hook-form";
import type { RegisterFields } from "@/schemas/register";
import {useTranslation} from "react-i18next";
import {CircleX} from "lucide-react";
import {DOCUMENT_TYPE_VALUES} from "@/schemas/core/enums.ts";
import {Input} from "@/components/ui/input.tsx";
import {Checkbox} from "@/components/ui/checkbox.tsx";
import {Label} from "@/components/ui/label.tsx";

type Props = {
    form: UseFormReturn<RegisterFields>;
};

export const Step3Address = ({ form }: Props) => {
    const { t } = useTranslation("auth");

    const {
        register,
        formState: { errors },
    } = form;

    return (
        <div className="space-y-4">

            <div className="flex gap-10">
                <div>
                    <label
                        htmlFor="documentType"
                        className="block text-sm font-medium text-casinoapp-black"
                    >
                        {t("register.documentType")}
                    </label>
                    <select
                        id="documentType"
                        {...register("documentType")}
                        className="h-10 w-35 rounded-md bg-white border-1
                            border-zinc-300 focus:outline-none
                            focus-visible:border-zinc-400 px-4 py-2 mt-0.5"
                    >
                        <option value="">Select</option>
                        {DOCUMENT_TYPE_VALUES.map((d) => (
                            <option key={d} value={d}>
                                {t(`register.documentTypes.${d}`)}
                            </option>
                        ))}
                    </select>
                    {errors.documentType && (
                        <div className="flex items-center gap-1 mt-2 text-xs text-red-500">
                            <CircleX className="h-4 w-4" />
                            <span>
                                {t(errors.documentType.message as string)}
                            </span>
                        </div>
                    )}
                </div>

                <div>
                    <label
                        htmlFor="documentNumber"
                        className="block text-sm font-medium text-casinoapp-black"
                    >
                        {t("register.documentNumber")}
                    </label>
                    <Input
                        id="documentNumber"
                        {...register("documentNumber")}
                        autoComplete="off"
                        placeholder={t("register.documentNumberInput")}
                        className={`
                            w-45 mt-0.5
                            focus-visible:ring-0
                            focus-visible:ring-offset-0
                        ${
                            errors.documentNumber
                                ? "border-red-500 focus-visible:border-red-500"
                                : "border-zinc-300 focus-visible:border-zinc-400"
                        }
                    `}
                    />
                    {errors.documentNumber && (
                        <div className="flex items-center gap-1 mt-2 text-xs text-red-500">
                            <CircleX className="h-4 w-4" />
                            <span className="max-w-40">
                                {t(errors.documentNumber.message as string)}
                            </span>
                        </div>
                    )}
                </div>
            </div>

            <div>
                <label
                    htmlFor="streetName"
                    className="block text-sm font-medium text-casinoapp-black"
                >
                    {t("register.streetName")}
                </label>
                <Input
                    id="streetName"
                    {...register("streetName")}
                    autoComplete="address-line1"
                    placeholder={t("register.streetNameInput")}
                    className={`
                            w-90 mt-0.5
                            focus-visible:ring-0
                            focus-visible:ring-offset-0
                        ${
                        errors.streetName
                            ? "border-red-500 focus-visible:border-red-500"
                            : "border-zinc-300 focus-visible:border-zinc-400"
                    }
                    `}
                />
                {form.formState.touchedFields.streetName && errors.streetName && (
                    <div className="flex items-center gap-1 mt-2 text-xs text-red-500">
                        <CircleX className="h-4 w-4" />
                        <span>
                            {t(errors.streetName.message as string)}
                        </span>
                    </div>
                )}
            </div>

            <div className="flex gap-4">
                <div>
                    <label
                        htmlFor="streetNumber"
                        className="block text-sm font-medium text-casinoapp-black"
                    >
                        {t("register.streetNumber")}
                    </label>
                    <Input
                        id="streetNumber"
                        {...register("streetNumber")}
                        autoComplete="address-line2"
                        placeholder={t("register.streetNumberInput")}
                        className={`
                            w-43 mt-0.5
                            focus-visible:ring-0
                            focus-visible:ring-offset-0
                        ${
                            errors.streetNumber
                                ? "border-red-500 focus-visible:border-red-500"
                                : "border-zinc-300 focus-visible:border-zinc-400"
                        }
                    `}
                    />
                    {form.formState.touchedFields.streetNumber && errors.streetNumber && (
                        <div className="flex items-center gap-1 mt-2 text-xs text-red-500">
                            <CircleX className="h-4 w-4" />
                            <span className="max-w-36">
                                {t(errors.streetNumber.message as string)}
                            </span>
                        </div>
                    )}
                </div>

                <div>
                    <label
                        htmlFor="postalCode"
                        className="block text-sm font-medium text-casinoapp-black"
                    >
                        {t("register.postalCode")}
                    </label>
                    <Input
                        id="postalCode"
                        {...register("postalCode")}
                        autoComplete="postal-code"
                        placeholder={t("register.postalCodeInput")}
                        className={`
                            w-43 mt-0.5
                            focus-visible:ring-0
                            focus-visible:ring-offset-0
                        ${
                            errors.postalCode
                                ? "border-red-500 focus-visible:border-red-500"
                                : "border-zinc-300 focus-visible:border-zinc-400"
                        }
                    `}
                    />
                    {form.formState.touchedFields.postalCode && errors.postalCode && (
                        <div className="flex items-center gap-1 mt-2 text-xs text-red-500">
                            <CircleX className="h-4 w-4" />
                            <span className="max-w-36">
                                {t(errors.postalCode.message as string)}
                            </span>
                        </div>
                    )}
                </div>
            </div>

            <div>
                <label
                    htmlFor="city"
                    className="block text-sm font-medium text-casinoapp-black"
                >
                    {t("register.city")}
                </label>
                <Input
                    id="city"
                    {...register("city")}
                    autoComplete="address-level2"
                    placeholder={t("register.cityInput")}
                    className={`
                            w-90 mt-0.5
                            focus-visible:ring-0
                            focus-visible:ring-offset-0
                        ${
                        errors.city
                            ? "border-red-500 focus-visible:border-red-500"
                            : "border-zinc-300 focus-visible:border-zinc-400"
                    }
                    `}
                />
                {form.formState.touchedFields.city && errors.city && (
                    <div className="flex items-center gap-1 mt-2 text-xs text-red-500">
                        <CircleX className="h-4 w-4 " />
                        <span>
                            {t(errors.city.message as string)}
                        </span>
                    </div>
                )}
            </div>

            <div>
                <div className="text-sm font-medium text-casinoapp-black">
                    {t("register.countryCode")}
                </div>

                <div className="w-90 rounded-md bg-casinoapp-light-gray/40 px-4 py-2 mt-0.5">
                    <p className="text-casinoapp-black">
                        {t("register.countryCodeInput")}
                    </p>
                </div>
            </div>

            <div className="space-y-2">
                <div className="flex items-center gap-1">
                    <Checkbox
                        id="ageVerified"
                        checked={form.watch("isAgeVerified")}
                        onCheckedChange={(checked) =>
                            form.setValue("isAgeVerified", checked === true, {
                                shouldDirty: true,
                                shouldTouch: true,
                                shouldValidate: true,
                            })
                        }
                        className="border-zinc-400
                            data-[state=checked]:bg-casinoapp-dark-blue
                            data-[state=checked]:border-casinoapp-dark-blue"
                    />
                    <Label
                        htmlFor="ageVerified"
                        className="text-xs cursor-pointer"
                    >
                        {t("register.ageVerification")}
                    </Label>
                </div>
                {/*{form.formState.touchedFields.isAgeVerified && errors.isAgeVerified && (*/}
                {/*    <div className="flex items-center gap-1 mt-2 text-xs text-red-500">*/}
                {/*        <CircleX className="h-4 w-4" />*/}
                {/*        <span>*/}
                {/*            {t(errors.isAgeVerified.message as string)}*/}
                {/*        </span>*/}
                {/*    </div>*/}
                {/*)}*/}


                <div className="flex items-center gap-1">
                    <Checkbox
                        id="termsAccepted"
                        checked={form.watch("hasAcceptedTerms")}
                        onCheckedChange={(checked) =>
                            form.setValue("hasAcceptedTerms", checked === true, {
                                shouldDirty: true,
                                shouldTouch: true,
                                shouldValidate: true,
                            })
                        }
                        className="border-zinc-400
                            data-[state=checked]:bg-casinoapp-dark-blue
                            data-[state=checked]:border-casinoapp-dark-blue"
                    />
                    <Label
                        htmlFor="termsAccepted"
                        className="text-xs cursor-pointer max-w-85"
                    >
                        {t("register.termsAcceptance")}
                    </Label>
                </div>
                {/*{form.formState.touchedFields.hasAcceptedTerms && errors.hasAcceptedTerms && (*/}
                {/*    <div className="flex items-center gap-1 mt-2 text-xs text-red-500">*/}
                {/*        <CircleX className="h-4 w-4" />*/}
                {/*        <span>*/}
                {/*            {t(errors.hasAcceptedTerms.message as string)}*/}
                {/*        </span>*/}
                {/*    </div>*/}
                {/*)}*/}
            </div>
        </div>
    );
};
