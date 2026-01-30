import {amountSchema, parseEuroAmount} from "@/schemas/account/transactions/amount.ts"

export const withdrawSchema = (balance: number) =>
    amountSchema.superRefine((value, ctx) => {
        const amount = parseEuroAmount(value);

        if (amount <= 0) {
            ctx.addIssue({
                code: "custom",
                message: "errors.withdrawLimits",
            });
            return;
        }

        if (amount > balance) {
            ctx.addIssue({
                code: "custom",
                message: "errors.insufficientBalance",
            });
        }
    });