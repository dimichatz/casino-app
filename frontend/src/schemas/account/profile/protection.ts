import {z} from "zod";

export const passwordSchema = z.object({
    currentPassword: z
        .string()
        .min(1),
    newPassword: z
        .string()
        .min(8)
        .regex(
            /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8,50}$/
        )
})

export type UserChangePasswordDTO = z.infer<typeof passwordSchema>;