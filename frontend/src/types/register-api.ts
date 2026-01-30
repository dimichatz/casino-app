import type {KycStatus, UserRole} from "@/schemas/core/enums.ts";

export type { PlayerSignupDTO } from "@/schemas/register";

export type PlayerReadOnlyDTO = {
    id: string;
    username: string;
    email: string;
    userRole: UserRole;
    isActive: boolean;
    kycStatus: KycStatus;
    isKycVerified: boolean;
    isSelfExcluded: boolean;
    insertedAt: string;
};

export type PlayerSignupResponseDTO = {
    accessToken: string;
    user: PlayerReadOnlyDTO;
};