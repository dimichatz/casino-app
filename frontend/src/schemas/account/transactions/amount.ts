import { z } from "zod"


export const parseEuroAmount = (value: string): number =>
    Number(value.replace(/\./g, "").replace(",", "."))

export const amountSchema = z
    .string()
    .min(1, "errors.required")
    .refine(
        (value) => !Number.isNaN(parseEuroAmount(value)),
        "errors.invalid"
    )

