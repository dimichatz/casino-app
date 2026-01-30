import {useEffect, useRef, useState} from "react";
import GameSearch from "@/components/games/GameSearch.tsx";
import GameCard from "@/components/games/GameCard.tsx";
import GamesGrid from "@/components/games/GamesGrid.tsx";
import GamesSection from "@/components/games/GamesSection.tsx";
import {useTranslation} from "react-i18next";
import { useGames } from "@/hooks/useGames"
import NoGamesFoundCard from "@/components/games/NoGamesFoundCard.tsx";
import {useDocumentTitle} from "@/hooks/useDocumentTitle.ts";
import {useLocation} from "react-router-dom";
import GameCardSkeleton from "@/components/games/GameCardSkeleton.tsx";

const GamesPage = () => {
    const { t } = useTranslation("games");
    useDocumentTitle(t("common:documentTitle.casino"));
    const [search, setSearch] = useState("");
    const [debouncedSearch, setDebouncedSearch] = useState(search)
    const { games, loading, error, hasLoaded } = useGames(debouncedSearch);

    const searchRef = useRef<HTMLInputElement>(null);
    const location = useLocation();
    const shouldFocusSearch = location.state?.focusSearch === true;

    useEffect(() => {
        const timer = setTimeout(() => {
            setDebouncedSearch(search)
        }, 600) // delay in ms

        return () => clearTimeout(timer)
    }, [search]);

    useEffect(() => {
        if (shouldFocusSearch) {
            setTimeout(() => {
                searchRef.current?.focus();
                searchRef.current?.select();
            }, 0);

            window.history.replaceState({}, "");
        }
    }, [shouldFocusSearch, location.key]);


    return (
        <>
            <div className="container mx-auto px-4 pt-10">
                <div className="flex justify-center my-14">
                    <GameSearch
                        ref={searchRef}
                        value={search}
                        onChange={setSearch}
                    />
                </div>

                {error && (
                    <div className="text-center text-red-500">
                        {error}
                    </div>
                )}

                {loading && !hasLoaded && (
                    <GamesSection title={t("section")}>
                        <GamesGrid>
                            {Array.from({ length: 1 }).map((_, i) => (
                                <GameCardSkeleton key={i} />
                            ))}
                        </GamesGrid>
                    </GamesSection>
                )}

                {!loading && !error && hasLoaded && (
                    games.length === 0 ? (
                        <div className="text-center text-muted-foreground py-10">
                            <NoGamesFoundCard/>
                        </div>
                        ) : (
                            <GamesSection title={t("section")}>
                                <GamesGrid>
                                        {games.map((game) => (
                                         <GameCard
                                            key={game.id}
                                            title={game.name}
                                            thumbnailUrl={game.thumbnailUrl}
                                         />
                                        ))}
                                </GamesGrid>
                            </GamesSection>
                        )
                    )
                }
            </div>
        </>
    );
};
export default GamesPage;