import { api } from "@/services/api";

export type PlayerBalance = {
    balance: number;
};

export const getPlayerBalance = async (
): Promise<PlayerBalance> => {
    const res =
        await api.get<PlayerBalance>("/player/balance");
    return res.data;
};