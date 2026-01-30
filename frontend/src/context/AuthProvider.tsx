import { useEffect, useState } from "react";
import { jwtDecode } from "jwt-decode";
import { getCookie, setCookie, deleteCookie } from "@/utils/cookies";
import { login } from "@/services/api.login";
import { AuthContext } from "@/context/AuthContext";
import type { UserLoginDTO } from "@/schemas/login";

type Role = "Player" | "Admin" | "SuperAdmin";

const ROLE_CLAIM =
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" as const;
const NAME_CLAIM =
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" as const;

type JwtPayload = {
    exp?: number;
    [ROLE_CLAIM]?: Role;
    [NAME_CLAIM]?: string;
};

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
    const [accessToken, setAccessToken] = useState<string | null>(null);
    const [role, setRole] = useState<Role | null>(null);
    const [username, setUsername] = useState<string | null>(null);
    const [loading, setLoading] = useState(true);

    const [logoutReason, setLogoutReason] = useState<
        "logout" | "expired" | null
    >(null);

    useEffect(() => {
        const token = getCookie("access_token");

        if (!token) {
            setLoading(false);
            return;
        }

        try {
            const decoded = jwtDecode<JwtPayload>(token);
            const now = Math.floor(Date.now() / 1000);

            // expiration check
            if (decoded.exp && decoded.exp < now) {
                deleteCookie("access_token");
                setLogoutReason("expired");
                setLoading(false);
                return;
            }

            setAccessToken(token);
            setRole(decoded[ROLE_CLAIM] ?? null);
            setUsername(decoded[NAME_CLAIM] ?? null);
        } catch {
            deleteCookie("access_token");
        } finally {
            setLoading(false);
        }
    }, []);

    const loginUser = async (fields: UserLoginDTO) => {
        const res = await login(fields);

        setCookie("access_token", res.token, {
            expires: 1,
            sameSite: "Lax",
            secure: false,
            path: "/",
        });

        const decoded = jwtDecode<JwtPayload>(res.token);

        setLogoutReason(null);
        setAccessToken(res.token);
        setRole(decoded[ROLE_CLAIM] ?? null);
        setUsername(decoded[NAME_CLAIM] ?? null);
    };

    const logoutUser = () => {
        deleteCookie("access_token");

        setLogoutReason("logout");
        setAccessToken(null);
        setRole(null);
        setUsername(null);
    };

    return (
        <AuthContext.Provider
            value={{
                isAuthenticated: !!accessToken,
                accessToken,
                role,
                username,
                loginUser,
                logoutUser,
                logoutReason,
                setLogoutReason,
                loading,
            }}
        >
            {loading ? null : children}
        </AuthContext.Provider>
    );
};
