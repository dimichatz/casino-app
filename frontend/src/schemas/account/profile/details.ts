import {z} from "zod";

export const emailSchema = z.object({
    email: z
        .email("details.email.errors.invalid")
});
export type UserUpdateEmailDTO = z.infer<typeof emailSchema>;

export const mobileSchema = z.object({
    phoneNumber: z
        .string()
        .regex(
            /^69\d{8}$/,
            "details.mobile.errors.format"
        )
});
export type UserUpdateMobileDTO = z.infer<typeof mobileSchema>;

export const addressSchema = z.object({
    streetName: z
        .string()
        .min(1, "auth:register.errors.streetName.required")
        .min(2, "auth:register.errors.streetName.length")
        .max(100, "auth:register.errors.streetName.length")
        .regex(/^[a-zA-Z0-9 -]*$/, "auth:register.errors.streetName.format"),

    streetNumber: z
        .string()
        .min(1, "auth:register.errors.streetNumber.required")
        .max(20, "auth:register.errors.streetNumber.length")
        .regex(/^[a-zA-Z0-9 -]*$/, "auth:register.errors.streetNumber.format"),

    postalCode: z
        .string()
        .min(1, "auth:register.errors.postalCode.required")
        .min(4, "auth:register.errors.postalCode.length")
        .max(10, "auth:register.errors.postalCode.length")
        .regex(/^[a-zA-Z0-9 -]*$/, "auth:register.errors.postalCode.format"),

    city: z
        .string()
        .min(1, "auth:register.errors.city.required")
        .min(2, "auth:register.errors.city.length")
        .max(60, "auth:register.errors.city.length")
        .regex(/^[a-zA-Z -]*$/, "auth:register.errors.city.format"),
});
export type UserUpdateAddressDTO = z.infer<typeof addressSchema>;