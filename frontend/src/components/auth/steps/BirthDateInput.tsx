import IMask from "imask";
import { IMaskInput } from "react-imask";
import {useEffect, useMemo, useRef, useState} from "react";

type Props = {
    id?: string;
    value?: string; // YYYY-MM-DD
    onChange: (value: string) => void;
    error?: boolean;
};


const buildGhostDate = (raw: string) => {
    const template = "DD/MM/YYYY";
    const digits = raw.replace(/\D/g, "");
    let i = 0;

    return template.replace(/[DMY]/g, (ch) => digits[i++] ?? ch);
};

export function CasinoBirthDateInput({ id, value, onChange, error }: Props) {
    const [raw, setRaw] = useState("");

    const lastValueRef = useRef<string | undefined>(undefined);

    useEffect(() => {
        if (!value || value === lastValueRef.current) return;

        lastValueRef.current = value;

        const [y, m, d] = value.split("-");
        setRaw(`${d}/${m}/${y}`);
    }, [value]);

    const ghost = useMemo(() => buildGhostDate(raw), [raw]);

    return (
        <div className="relative w-90 h-10">
            {/* Ghost */}
            <div
                className="absolute inset-0 px-4 py-2 pointer-events-none
                    select-none whitespace-pre flex bg-white"
            >
                {ghost.split("").map((char, idx) => {
                    const shouldHide = raw.length > idx;

                    return (
                        <span
                            key={idx}
                            className={shouldHide ? "text-transparent" : "text-zinc-400"}
                        >
                            {char}
                        </span>
                    );
                })}
            </div>

            {/* Real input */}
            <IMaskInput
                id={id}
                value={raw}
                mask="d/m/y"
                lazy
                autofix={false}
                inputMode="numeric"
                blocks={{
                    d: {
                        mask: IMask.MaskedRange,
                        from: 1,
                        to: 31,
                        maxLength: 2,
                        autofix: false,
                    },
                    m: {
                        mask: IMask.MaskedRange,
                        from: 1,
                        to: 12,
                        maxLength: 2,
                        autofix: false,
                    },
                    y: {
                        mask: IMask.MaskedRange,
                        from: 1900,
                        to: new Date().getFullYear(),
                        maxLength: 4,
                        autofix: false,
                    },
                }}
                onAccept={(val) => {
                    setRaw(val);

                    const [d, m, y] = val.split("/");
                    if (d?.length === 2 && m?.length === 2 && y?.length === 4) {
                        onChange(`${y}-${m}-${d}`);
                    } else {
                        onChange("");
                    }
                }}
                className={`
                        relative z-10 w-full h-full px-4 py-2
                        bg-transparent text-black caret-black
                        border rounded-md focus:outline-none
                    ${error ? "border-red-500" : "border-zinc-300"}
                `}
            />
        </div>
    );
}
