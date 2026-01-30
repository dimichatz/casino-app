import { api } from "@/services/api";
import type {PlayerSignupDTO, PlayerSignupResponseDTO} from "@/types/register-api";

export const registerPlayer = async (
    payload: PlayerSignupDTO
): Promise<PlayerSignupResponseDTO> => {
    const response = await api.post<PlayerSignupResponseDTO>(
        "/auth/signup",
        payload
    );

    return response.data;
};