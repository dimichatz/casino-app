import type {ReactNode} from "react";

const GamesSection = ({ title, children }: { title: string; children: ReactNode }) => {
    return (
        <section className="mt-20">
            <h2 className="text-2xl font-bold text-white mb-8">
                {title}
            </h2>
            {children}
        </section>

    );
};

export default GamesSection;