import type {UserLoginDTO} from "@/schemas/login.ts";
import { api } from "@/services/api";

export type LoginResponse = {
    token: string;
}

export const login = async (
    payload: UserLoginDTO
): Promise<LoginResponse> => {
    const res = await api.post<LoginResponse>(
        "/auth/login",
        payload
    );

    return res.data;
};