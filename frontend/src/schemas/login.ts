import {z} from "zod";

export const loginSchema = z.object({
    usernameOrEmail: z.string(),
    password: z.string()
});

export type UserLoginDTO = z.infer<typeof loginSchema>;