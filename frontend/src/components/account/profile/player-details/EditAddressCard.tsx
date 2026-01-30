import {useTranslation} from "react-i18next";
import EditableRow from "@/components/account/profile/player-details/EditableRow.tsx";
import {addressSchema, type UserUpdateAddressDTO} from "@/schemas/account/profile/details.ts";
import {Form, FormControl, FormField, FormItem, FormLabel} from "@/components/ui/form.tsx";
import {Input} from "@/components/ui/input.tsx";
import {CircleX} from "lucide-react";
import {Button} from "@/components/ui/button.tsx";
import {useForm, useWatch} from "react-hook-form";
import {zodResolver} from "@hookform/resolvers/zod";
import {z} from "zod";
import {updatePlayer} from "@/services/api.player.ts";
import {toast} from "sonner";
import {useEffect, useMemo} from "react";

interface EditAddressCardProps {
    address?: {
        streetName?: string;
        streetNumber?: string;
        postalCode?: string;
        city?: string;
    }
    onUpdated?: () => Promise<void>;
    open: boolean;
    disabled: boolean;
    onToggle: () => void;
}

type AddressFormValues = z.infer<typeof addressSchema>;

const EditAddressCard = ({
                             address,
                             onUpdated,
                             open,
                             disabled,
                             onToggle,
                         }: EditAddressCardProps) => {
    const { t } = useTranslation("profile");

    const addressValue = address
        ? [
            [address.streetName, address.streetNumber].filter(Boolean).join(" "),
            address.postalCode,
            address.city,
        ].filter(Boolean).join(", ")
        : undefined;

    const form = useForm<AddressFormValues>({
        resolver: zodResolver(addressSchema),
            defaultValues: {
                streetName: address?.streetName?.trim() ?? "",
                streetNumber: address?.streetNumber?.trim() ?? "",
                postalCode: address?.postalCode?.trim() ?? "",
                city: address?.city?.trim() ?? "",
        },
        mode: "onChange",
    });

    const normalize = (v: unknown) =>
        typeof v === "string" ? v.trim() : v;

    const currentValues = useWatch({ control: form.control });

    const defaultValues = useMemo(
        () => ({
            streetName: address?.streetName ?? "",
            streetNumber: address?.streetNumber ?? "",
            postalCode: address?.postalCode ?? "",
            city: address?.city ?? "",
        }),
        [address]
    );

    useEffect(() => {
        if (open) {
            form.reset(defaultValues);
        }
    }, [open, defaultValues, form]);

    const hasMeaningfulChanges = useMemo(() => {
        const keys = Object.keys(defaultValues) as (keyof typeof defaultValues)[];
        return keys.some((k) =>
            normalize(currentValues?.[k]) !== normalize(defaultValues[k]));
    }, [currentValues, defaultValues]);

    const isDisabled =
        disabled ||
        !hasMeaningfulChanges ||
        form.formState.isSubmitting ||
        !form.formState.isValid;

    const onSubmit = async (data: AddressFormValues) => {
        try {
            const dto: UserUpdateAddressDTO = {
                streetName: data.streetName,
                streetNumber: data.streetNumber,
                postalCode: data.postalCode,
                city: data.city
            };

            await updatePlayer({
                addressDetails: dto,
            });
            await onUpdated?.();

            toast.success(t("details.address.success"));
            onToggle();
        } catch (err: unknown) {
            console.warn("Request rejected:", err);
            toast.error(t("details.errors.generic"));
        }
    };

    return (
        <EditableRow
            label={t("details.address.title")}
            value={addressValue}
            helperText={t("details.address.helper")}
            open={open}
            onToggle={onToggle}
            disabled={disabled}
            rounded="bottom"
        >
            <div className="flex items-center">
                <Form {...form}>
                    <form
                        onSubmit={form.handleSubmit(onSubmit)}
                    >
                        <div className="flex flex-col p-4 gap-4 border rounded-sm border-zinc-150">
                            {/*STREET NAME*/}
                            <div>
                                <FormField
                                    control={form.control}
                                    name="streetName"
                                    render={({ field }) => {
                                        const hasError = !!form.formState.errors.streetName;

                                        return (
                                            <FormItem>
                                                <FormLabel className="text-casinoapp-black">
                                                    {t("auth:register.streetName")}
                                                </FormLabel>
                                                <FormControl>
                                                    <Input
                                                        {...field}
                                                        type="text"
                                                        autoComplete="address-line1"
                                                        placeholder={t("auth:register.streetNameInput")}
                                                        onBlur={(e) => {
                                                            field.onChange(e.target.value.trim());
                                                            field.onBlur();
                                                        }}
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
                                {form.formState.errors.streetName?.message && (
                                    <div className="flex items-center gap-1 mt-2 text-xs text-red-500">
                                        <CircleX className="h-4 w-4" />
                                        <span>
                                        {t(form.formState.errors.streetName.message)}
                                    </span>
                                    </div>
                                )}
                            </div>

                            {/*STREET NUMBER*/}
                            <div>
                                <FormField
                                    control={form.control}
                                    name="streetNumber"
                                    render={({ field }) => {
                                        const hasError = !!form.formState.errors.streetNumber;

                                        return (
                                            <FormItem>
                                                <FormLabel className="text-casinoapp-black">
                                                    {t("auth:register.streetNumber")}
                                                </FormLabel>
                                                <FormControl>
                                                    <Input
                                                        {...field}
                                                        type="text"
                                                        autoComplete="address-line1"
                                                        placeholder={t("auth:register.streetNumberInput")}
                                                        onBlur={(e) => {
                                                            field.onChange(e.target.value.trim());
                                                            field.onBlur();
                                                        }}
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
                                {form.formState.errors.streetNumber?.message && (
                                    <div className="flex items-center gap-1 mt-2 text-xs text-red-500">
                                        <CircleX className="h-4 w-4" />
                                        <span>
                                        {t(form.formState.errors.streetNumber.message)}
                                    </span>
                                    </div>
                                )}
                            </div>

                            {/*POSTAL CODE*/}
                            <div>
                                <FormField
                                    control={form.control}
                                    name="postalCode"
                                    render={({ field }) => {
                                        const hasError = !!form.formState.errors.postalCode;

                                        return (
                                            <FormItem>
                                                <FormLabel className="text-casinoapp-black">
                                                    {t("auth:register.postalCode")}
                                                </FormLabel>
                                                <FormControl>
                                                    <Input
                                                        {...field}
                                                        type="text"
                                                        autoComplete="postal-code"
                                                        placeholder={t("auth:register.postalCodeInput")}
                                                        onBlur={(e) => {
                                                            field.onChange(e.target.value.trim());
                                                            field.onBlur();
                                                        }}
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
                                {form.formState.errors.postalCode?.message && (
                                    <div className="flex items-center gap-1 mt-2 text-xs text-red-500">
                                        <CircleX className="h-4 w-4" />
                                        <span>
                                        {t(form.formState.errors.postalCode.message)}
                                    </span>
                                    </div>
                                )}
                            </div>

                            {/*CITY*/}
                            <div>
                                <FormField
                                    control={form.control}
                                    name="city"
                                    render={({ field }) => {
                                        const hasError = !!form.formState.errors.city;

                                        return (
                                            <FormItem>
                                                <FormLabel className="text-casinoapp-black">
                                                    {t("auth:register.city")}
                                                </FormLabel>
                                                <FormControl>
                                                    <Input
                                                        {...field}
                                                        type="text"
                                                        autoComplete="address-level2"
                                                        placeholder={t("auth:register.cityInput")}
                                                        onBlur={(e) => {
                                                            field.onChange(e.target.value.trim());
                                                            field.onBlur();
                                                        }}
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
                                {form.formState.errors.city?.message && (
                                    <div className="flex items-center gap-1 mt-2 text-xs text-red-500">
                                        <CircleX className="h-4 w-4" />
                                        <span>
                                        {t(form.formState.errors.city.message)}
                                    </span>
                                    </div>
                                )}
                            </div>

                            {/*COUNTRY CODE DEFAULT*/}
                            <div>
                                <span className="font-medium text-sm cursor-default">
                                    {t("auth:register.countryCode")}
                                </span>
                                <Input
                                    name="countryCode"
                                    value={t("auth:register.countryCodeInput")}
                                    readOnly
                                    type="text"
                                    autoComplete="country-name"
                                    className="
                                    !cursor-default
                                    select-none
                                    pointer-events-none
                                    focus-visible:ring-0
                                    focus-visible:ring-offset-0
                                    focus-visible:border-zinc-400
                                    bg-muted"
                                />
                            </div>

                            <Button
                                className="w-80 mt-2 text-zinc-200 bg-casinoapp-black/95
                                hover:bg-casinoapp-black hover:text-white cursor-pointer"
                                type="submit"
                                disabled={isDisabled}
                            >
                                {t("details.save")}
                            </Button>
                        </div>

                    </form>
                </Form>
            </div>
        </EditableRow>
    );
};

export default EditAddressCard;