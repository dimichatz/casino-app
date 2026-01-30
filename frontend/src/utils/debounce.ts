export function debounce(
    fn: (value: string) => Promise<void> | void,
    delay = 500
): (value: string) => void {
    let timeout: ReturnType<typeof setTimeout>;

    return (value: string) => {
        clearTimeout(timeout);
        timeout = setTimeout(() => {
            void fn(value);
        }, delay);
    };
}
