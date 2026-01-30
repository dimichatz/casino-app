import { createContext } from "react";
import type { UserLoginDTO } from "@/schemas/login";

export type Role = "Player" | "Admin" | "SuperAdmin";
export type LogoutReason = "logout" | "expired" | null;

type AuthContextProps = {
    isAuthenticated: boolean;
    accessToken: string | null;

    role: Role | null;
    username: string | null;

    loginUser: (fields: UserLoginDTO) => Promise<void>;
    logoutUser: () => void;
    loading: boolean;

    logoutReason: LogoutReason;
    setLogoutReason: React.Dispatch<React.SetStateAction<LogoutReason>>;
};

export const AuthContext =
    createContext<AuthContextProps | undefined>(undefined);