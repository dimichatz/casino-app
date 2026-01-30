import { createContext } from "react";

export type PlayerContextProps = {
    balance: number | null;
    setBalance: (balance: number | null) => void;
    refreshBalance: () => Promise<void>;
};

export const PlayerContext =
    createContext<PlayerContextProps | undefined>(undefined);