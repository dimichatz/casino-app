import { Input } from "@/components/ui/input";
import { Search, X } from "lucide-react";
import {forwardRef, useImperativeHandle, useRef, useState} from "react";
import {useTranslation} from "react-i18next";


type GameSearchProps = {
    value?: string;
    onChange?: (value: string) => void;
};

const GameSearch = forwardRef<HTMLInputElement, GameSearchProps>(
    ({ value = "", onChange }, ref) => {
    const { t } = useTranslation("games");
    const [internalValue, setInternalValue] = useState(value);
    const inputRef = useRef<HTMLInputElement>(null);

    const controlled = onChange !== undefined;
    const inputValue = controlled ? value : internalValue;

    useImperativeHandle(ref, () => inputRef.current!, []);

    const handleChange = (v: string) => {
        if (controlled) {
            onChange(v);
        } else {
            setInternalValue(v);
        }
    };

    const clearInput = () => {
        handleChange("");
        inputRef.current?.focus();
    };


    return (
        <div className="relative w-full max-w-md">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground h-5 w-5"/>

            <Input
                name="gameSearch"
                ref={inputRef}
                value={inputValue}
                onChange={(e) => handleChange(e.target.value)}
                placeholder={t("searchbar")}
                className="pl-10 pr-10"
            />

            {inputValue && (
                <button
                    type="button"
                    onClick={clearInput}
                    className="absolute cursor-pointer right-3 top-1/2 -translate-y-1/2 text-muted-foreground
                         hover:text-foreground"
                >
                    <X className="h-5 w-5" />
                </button>
            )}

        </div>
    );
});

GameSearch.displayName = "GameSearch";

export default GameSearch;