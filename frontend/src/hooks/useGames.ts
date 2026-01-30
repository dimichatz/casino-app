import { useEffect, useState } from "react"
import { getGames } from "@/services/api.games"

export type Game = {
    id: string
    name: string
    thumbnailUrl: string
}

// const delay = (ms: number) =>
//     new Promise(resolve => setTimeout(resolve, ms))

export const useGames = (search: string) => {
    const [games, setGames] = useState<Game[]>([])
    const [loading, setLoading] = useState(false)
    const [error, setError] = useState<string | null>(null)
    const [hasLoaded, setHasLoaded] = useState(false)

    useEffect(() => {
        let cancelled = false

        if (!hasLoaded) {
            setLoading(true)
        }
        setError(null)

        const load = async () => {
            try {
                // await delay(1000);

                const { games } = await getGames({
                    pageNumber: 1,
                    pageSize: 9,
                    search,
                })

                if (!cancelled) {
                    setGames(games)
                }
            } catch {
                if (!cancelled) {
                    setError("Failed to load games")
                }
            } finally {
                if (!cancelled) {
                    setLoading(false)
                    setHasLoaded(true)
                }
            }
        }

        void load()

        return () => {
            cancelled = true
        }
    }, [search, hasLoaded])

    return { games, loading, error, hasLoaded }
}