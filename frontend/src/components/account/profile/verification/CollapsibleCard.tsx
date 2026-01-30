import {type ReactNode, useState} from "react";
import { Card, CardContent } from "@/components/ui/card.tsx";
import {ChevronRight} from "lucide-react";
import { cn } from "@/lib/utils.ts";


interface CollapsibleCardProps {
    title: ReactNode;
    children: ReactNode;
    defaultOpen?: boolean;
}

const CollapsibleCard = ({
                             title,
                             children,
                             defaultOpen = false,
                         }: CollapsibleCardProps) => {
    const [open, setOpen] = useState(defaultOpen);

    return (
        <Card
            className={cn(
                "border-zinc-300 rounded-2xl"
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
                        className="px-10 py-6 shadow-[inset_0_2px_4px_-2px_rgba(0,0,0,0.35)]"
                    >
                        {children}
                    </CardContent>
                </div>
            </div>
        </Card>
    );
};

export default CollapsibleCard;
