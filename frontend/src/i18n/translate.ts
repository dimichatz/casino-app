import i18n from "./index";

export const t = (key: string, params?: Record<string, unknown>) =>
    i18n.t(key, params);
