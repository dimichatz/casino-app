import { Skeleton } from "@/components/ui/skeleton";

const TransactionRowSkeleton = () => {
    return (
        <div
            className="
                grid lg:grid-cols-[2fr_1fr_1fr_1fr_1fr_auto] grid-cols-[2fr_1fr] items-center gap-x-12
             rounded-lg bg-white p-4 text-sm text-casinoapp-black shadow-sm"
        >

            <div className="grid grid-cols-[1fr_2fr] items-center">
                <Skeleton className="h-9 w-8 rounded-full" />

                <div className="flex flex-col gap-1 overflow-hidden">
                    <Skeleton className="h-3 w-14 lg:w-18" />
                    <Skeleton className="h-3 w-14 lg:w-18" />
                </div>
            </div>

            <Skeleton className="hidden lg:block h-6 w-17" />
            <Skeleton className="hidden lg:block h-6 w-17" />
            <Skeleton className="h-6 w-13 lg:w-30" />
            <Skeleton className="hidden lg:block h-6 w-17 ml-auto" />
            <Skeleton className="hidden lg:block h-4 w-4" />
        </div>
    );
};

export default TransactionRowSkeleton;
