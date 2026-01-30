import type {ReactNode} from "react";

const GamesGrid = ({ children }: { children: ReactNode }) => {
    return (
        <div className="grid grid-cols-3 gap-4">
            {children}
        </div>

    );
};

export default GamesGrid;