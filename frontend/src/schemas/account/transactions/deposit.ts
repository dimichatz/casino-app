import {amountSchema, parseEuroAmount} from "@/schemas/account/transactions/amount.ts"

export const depositSchema = amountSchema
    .refine((value) => {
        const amount = parseEuroAmount(value)
        return amount >= 5 && amount <= 10000
    }, "errors.depositLimits");
