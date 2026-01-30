import {type ReactNode, useState} from "react";
import { Card, CardContent } from "@/components/ui/card.tsx";
import {ChevronRight} from "lucide-react";
import { cn } from "@/lib/utils.ts";


type RoundedVariant = "top" | "bottom" | "none" | "all";

interface CollapsibleCardProps {
    title: string;
    children: ReactNode;
    defaultOpen?: boolean;
    rounded?: RoundedVariant;
}

const roundedClasses: Record<RoundedVariant, string> = {
    all: "rounded-2xl",
    top: "rounded-t-2xl rounded-b-none",
    bottom: "rounded-b-2xl rounded-t-none",
    none: "rounded-none",
};

const CollapsibleCard = ({
                             title,
                             children,
                             defaultOpen = false,
                             rounded = "all",
                         }: CollapsibleCardProps) => {
    const [open, setOpen] = useState(defaultOpen);

    return (
        <Card
            className={cn(
                "border-casinoapp-black",
                roundedClasses[rounded]
            )}
        >
            <button
                type="button"
                aria-expanded={open}
                onClick={() => setOpen(!open)}
                className="flex w-full items-center justify-between px-4 py-6
                   text-base font-semibold text-left cursor-pointer"
            >
                <span>{title}</span>

                <ChevronRight
                    className={cn(
                        "h-6 w-6 transition-transform duration-300",
                        open && "rotate-90"
                    )}
                />
            </button>

            <div
                className={cn(
                    "grid transition-all duration-300",
                    open ? "grid-rows-[1fr]" : "grid-rows-[0fr]"
                )}
            >
                <div className="overflow-hidden">
                    <CardContent
                        className="px-10 pb-6 pt-0 shadow-[inset_0_2px_4px_-2px_rgba(0,0,0,0.35)]"
                    >
                        {children}
                    </CardContent>
                </div>
            </div>
        </Card>
    );
};

export default CollapsibleCard;
