import {z} from "zod";
import {DOCUMENT_TYPE_VALUES, GENDER_VALUES} from "@/schemas/core/enums.ts";
import { isAtLeastAge } from "@/schemas/validators/minimumAge";
import {isAtMostAge} from "@/schemas/validators/maximumAge.ts";

export const registerSchema = z.object({
// step 1
    email: z
        .string()
        .nonempty("register.errors.email.required")
        .max(60, "register.errors.email.max")
        .refine(
            (val) => z.email().safeParse(val).success,
            { message: "register.errors.email.invalid" }
        ),
    username: z
        .string()
        .min(1, "register.errors.username.required")
        .min(4, "register.errors.username.length")
        .max(20, "register.errors.username.length")
        .regex(/^[a-z0-9]*$/, "register.errors.username.format"),
    password: z
        .string()
        .min(1, "register.errors.password.required")
        .regex(
            /^[A-Za-z\d!@#$%^&*]+$/,
            "register.errors.password.invalidCharacters"
        )
        .regex(
            /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*]).{8,}$/,
            "register.errors.password.strength"
        ),
    confirmPassword: z
        .string()
        .min(1, "register.errors.confirmPassword.required"),

    // step 2
    phoneNumber: z
        .string()
        .min(1, "register.errors.phoneNumber.required")
        .regex(
            /^69\d{8}$/,
            "register.errors.phoneNumber.format"
        ),
    firstname: z
        .string()
        .min(1, "register.errors.firstname.required")
        .min(2, "register.errors.firstname.length")
        .max(60, "register.errors.firstname.length")
        .regex(/^[a-zA-Z -]*$/, "register.errors.firstname.format"),

    lastname: z
        .string()
        .min(1, "register.errors.lastname.required")
        .min(2, "register.errors.lastname.length")
        .max(60, "register.errors.lastname.length")
        .regex(/^[a-zA-Z -]*$/, "register.errors.lastname.format"),

    gender: z.enum(GENDER_VALUES, {
        message: "register.errors.gender.invalid",
    }),

    birthDate: z
        .string()
        .min(1, "register.errors.birthDate.required")
        .regex(/^\d{4}-\d{2}-\d{2}$/, "register.errors.birthDate.format")
        .refine(
            (value) => isAtLeastAge(new Date(value), 21),
            { message: "register.errors.birthDate.ageMin" }
        )
        .refine(
            (value) => isAtMostAge(new Date(value), 100),
            { message: "register.errors.birthDate.ageMax" }
        ),

    // step 3
    documentType: z.enum(DOCUMENT_TYPE_VALUES, {
        message: "register.errors.documentType.invalid",
    }),

    documentNumber: z
        .string()
        .min(1, "register.errors.documentNumber.required")
        .min(5, "register.errors.documentNumber.length")
        .max(25, "register.errors.documentNumber.length")
        .regex(/^[a-zA-Z0-9 -]*$/, "register.errors.documentNumber.format"),

    streetName: z
        .string()
        .min(1, "register.errors.streetName.required")
        .min(2, "register.errors.streetName.length")
        .max(100, "register.errors.streetName.length")
        .regex(/^[a-zA-Z0-9 -]*$/, "register.errors.streetName.format"),

    streetNumber: z
        .string()
        .min(1, "register.errors.streetNumber.required")
        .max(20, "register.errors.streetNumber.length")
        .regex(/^[a-zA-Z0-9 -]*$/, "register.errors.streetNumber.format"),

    postalCode: z
        .string()
        .min(1, "register.errors.postalCode.required")
        .min(4, "register.errors.postalCode.length")
        .max(10, "register.errors.postalCode.length")
        .regex(/^[a-zA-Z0-9 -]*$/, "register.errors.postalCode.format"),

    city: z
        .string()
        .min(1, "register.errors.city.required")
        .min(2, "register.errors.city.length")
        .max(60, "register.errors.city.length")
        .regex(/^[a-zA-Z -]*$/, "register.errors.city.format"),

    // countryCode: z
    //     .string()
    //     .min(1, "register.errors.countryCode.required")
    //     .length(2, "register.errors.countryCode.invalid"),

    isAgeVerified: z.boolean().refine((v) => v === true, {
        message: "register.errors.isAgeVerified.required",
    }),

    hasAcceptedTerms: z.boolean().refine((v) => v === true, {
        message: "register.errors.hasAcceptedTerms.required",
    }),
});

export type RegisterFields = z.infer<typeof registerSchema>;

export const step1Schema = z
    .object({
        email: registerSchema.shape.email,
        username: registerSchema.shape.username,
        password: registerSchema.shape.password,
        confirmPassword: registerSchema.shape.confirmPassword,
    })
    .superRefine((data, ctx) => {
        if (data.confirmPassword && data.password !== data.confirmPassword) {
            ctx.addIssue({
                code: "custom",
                path: ["confirmPassword"],
                message: "register.errors.password.mismatch",
            });
        }
    });

export const step2Schema = registerSchema.pick({
    phoneNumber: true,
    firstname: true,
    lastname: true,
    gender: true,
    birthDate: true,
});

export const step3Schema = registerSchema.pick({
    documentType: true,
    documentNumber: true,
    streetName: true,
    streetNumber: true,
    postalCode: true,
    city: true,
    isAgeVerified: true,
    hasAcceptedTerms: true,
});

export const registerSubmitSchema = registerSchema.transform((data) => ({
    ...data,
    birthDate: new Date(`${data.birthDate}T00:00:00Z`).toISOString(),
    phoneNumber: `+30${data.phoneNumber}`,
    countryCode: "GR" as const,
}));

export type PlayerSignupDTO = z.infer<typeof registerSubmitSchema>;