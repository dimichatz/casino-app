import { api } from "@/services/api";

export const checkUsernameExists = async (
    username: string
): Promise<boolean> => {
    const res =
        await api.get<{ userExists: boolean }>("/auth/check/username", {
            params: { username }
        }
    );
    return res.data.userExists;
};

export const checkEmailExists = async (
    email: string
): Promise<boolean>  => {
    const res =
        await api.get<{ userExists: boolean }>("/auth/check/email", {
            params: { email }
        }
    );
    return res.data.userExists;
};

export const checkPhoneExists = async (
    phoneNumber: string
): Promise<boolean>  => {
    const res =
        await api.get<{ userExists: boolean }>( "/auth/check/phone", {
            params: { phoneNumber } }
    );
    return res.data.userExists;
};
