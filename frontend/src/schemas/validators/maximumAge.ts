export const isAtMostAge = (date: Date, maximumAge: number) => {
    const today = new Date();

    const maximumDate = new Date(
        today.getFullYear() - maximumAge,
        today.getMonth(),
        today.getDate()
    );
    return date >= maximumDate;
};