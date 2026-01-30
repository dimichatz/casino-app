import { api } from "@/services/api";
import type {UserChangePasswordDTO} from "@/types/password-api.ts";
import type {PlayerUpdateRequest} from "@/types/player-update-api.ts";
import type {PlayerFullDetails} from "@/types/player-read-api.ts";

export const changePlayerPassword = async (
    payload: UserChangePasswordDTO
    ) => {
        await api.put("/player/profile/password",
            payload
        );
};

export const getPlayer = async (): Promise<PlayerFullDetails> => {
    const { data } = await api.get<PlayerFullDetails>("/player/profile");
    return data;
};

export const updatePlayer = async (
    payload: PlayerUpdateRequest
): Promise<void> => {
    await api.put("/player/profile",
        payload
    );
};

export const uploadKycDocument = async (
    file: File
): Promise<void> => {
    const formData = new FormData();
    formData.append("file", file);
    await api.post("/player/profile/kyc/upload",
        formData
    );
};

export const getGameAccess = async () => {
    const response = await api.get("/player/access");
    return response.data as {
        hasGameAccess: boolean;
        reason?: string;
    };
};