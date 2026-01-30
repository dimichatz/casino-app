export const PASSWORD_ERROR_MAP: Record<string, string> = {
    CurrentPassword: "security.errors.currentPassword",
    NewPassword: "security.errors.newPassword",
};

export const EXCLUSION_ERROR_MAP: Record<string, string> = {
    PlayerPartialExclusion: "protection.exclusion.errors.playerPartialExclusion",
    PlayerPermanentExclusion: "protection.exclusion.errors.playerPermanentExclusion",
};

export const DETAILS_ERROR_MAP: Record<string, string> = {
    UserEmailAlreadyExists: "details.errors.userEmailAlreadyExists",
    UserMobileAlreadyExists: "details.errors.userMobileAlreadyExists",
};
