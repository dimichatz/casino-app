import {type ReactNode} from "react";
import { Card, CardContent } from "@/components/ui/card.tsx";
import { cn } from "@/lib/utils.ts";
import {useTranslation} from "react-i18next";


type RoundedVariant = "top" | "bottom" | "none" | "all";

interface EditableRowProps {
    label: string;
    value?: ReactNode;
    helperText?: string;
    children: ReactNode;
    open: boolean;
    onToggle: () => void;
    rounded?: RoundedVariant;
    disabled?: boolean;
}

const roundedClasses: Record<RoundedVariant, string> = {
    all: "rounded-2xl",
    top: "rounded-t-2xl rounded-b-none",
    bottom: "rounded-b-2xl rounded-t-none",
    none: "rounded-none",
};

const EditableRow = ({
                         label,
                         value,
                         children,
                         helperText,
                         open,
                         onToggle,
                         rounded = "all",
                         disabled = false,
                         }: EditableRowProps) => {
    const { t } = useTranslation("profile");

    return (
        <Card
            className={cn(
                "w-full max-w-4xl min-w-md border-casinoapp-black",
                roundedClasses[rounded]
            )}
        >
            <div
                className={cn(
                    "flex items-start justify-between px-4 py-5",
                    disabled && "opacity-50 pointer-events-none"
                )}
            >
                <div className="space-y-1">
                    <p className="text-sm font-semibold">{label}</p>

                    {!open && value && (
                        <p className="text-sm text-muted-foreground">
                            {value}
                        </p>
                    )}

                    {open && helperText && (
                        <p className="text-sm text-muted-foreground">
                            {helperText}
                        </p>
                    )}
                </div>

                <button
                    type="button"
                    aria-expanded={open}
                    disabled={disabled}
                    onClick={() => {
                        if (!disabled) {
                            onToggle();
                        }
                    }}
                    className={cn(
                        "text-sm font-medium",
                        disabled
                            ? "cursor-none text-zinc-400"
                            : "cursor-pointer text-casinoapp-black hover:text-casinoapp-dark-blue"
                    )}
                >
                    {open ? t("details.cancel") : t("details.edit")}
                </button>
            </div>

            <div
                className={cn(
                    "grid ",
                    open ? "grid-rows-[1fr]" : "grid-rows-[0fr]"
                )}
            >
                <div className="overflow-hidden">
                    <CardContent className="px-10 pb-6 pt-2">
                        {children}
                    </CardContent>
                </div>
            </div>
        </Card>
    );
};

export default EditableRow;
