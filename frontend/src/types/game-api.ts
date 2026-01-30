export type GameApiResponse = {
    id: string;
    name: string;
    code: string;
    description: string;
    imageUrl: string;
    minBet: number;
    maxBet: number;
    category: string;
    isEnabled: boolean;
}

export type PaginatedApiResponse<T> = {
    data: T[];
    totalRecords: number;
    pageNumber: number;
    pageSize: number;
    totalPages: number;
}