import {useCallback, useEffect, useState} from "react";
import { PlayerContext } from "@/context/PlayerContext";
import { useAuth } from "@/hooks/useAuth";
import { getPlayerBalance } from "@/services/api.player-balance.ts";
import * as React from "react";

export const PlayerProvider = ({ children }: { children: React.ReactNode }) => {
    const { isAuthenticated, role } = useAuth();
    const [balance, setBalance] = useState<number | null>(null);

    const refreshBalance = useCallback(async () => {
        if (!isAuthenticated || role !== "Player") return;

        const profile = await getPlayerBalance();
        setBalance(profile.balance);
    }, [isAuthenticated, role]);

    useEffect(() => {
        if (isAuthenticated && role === "Player") {
            void refreshBalance();
        } else {
            setBalance(null);
        }
    }, [isAuthenticated, role, refreshBalance]);

    return (
        <PlayerContext.Provider value={{ balance, setBalance, refreshBalance }}>
            {children}
        </PlayerContext.Provider>
    );
};