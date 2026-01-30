import { Card } from "@/components/ui/card";

const GameCardSkeleton = () => {
    return (
        <Card className="bg-transparent border-none shadow-none animate-pulse">

            <div className="relative overflow-hidden rounded-lg aspect-[16/9] bg-muted-foreground/20" />

            <div className="mt-2 flex justify-center">
                <div className="h-4 w-1/4 rounded bg-muted-foreground/20" />
            </div>
        </Card>
    );
};

export default GameCardSkeleton;
