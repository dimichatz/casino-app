import { api } from "@/services/api";
import type { GameApiResponse, PaginatedApiResponse } from "@/types/game-api";

type GetGamesParams = {
    pageNumber: number
    pageSize: number
    search?: string
}

export const getGames = async (
    params: GetGamesParams
) => {
    const response =
        await api.get<PaginatedApiResponse<GameApiResponse>>("/games", {
            params
        })

    return {
        games: response.data.data.map((g) => ({
            id: g.id,
            name: g.name,
            thumbnailUrl: g.imageUrl,
        })),
        totalRecords: response.data.totalRecords,
        pageNumber: response.data.pageNumber,
        pageSize: response.data.pageSize,
        totalPages: response.data.totalPages,
    }
}