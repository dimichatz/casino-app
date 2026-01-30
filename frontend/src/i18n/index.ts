import i18n from "i18next";
import { initReactI18next } from "react-i18next";

import commonEN from "./en/common.json";
import authEN from "./en/auth.json";
import gamesEN from "./en/games.json";
import accountEN from "./en/account.json";
import profileEN from "./en/profile.json";

import commonEL from "./el/common.json";
import authEL from "./el/auth.json";
import gamesEL from "./el/games.json";
import accountEL from "./el/account.json";
import profileEL from "./el/profile.json";

void i18n
    .use(initReactI18next)
    .init({
        resources: {
            en: {
                common: commonEN,
                auth: authEN,
                games: gamesEN,
                account: accountEN,
                profile: profileEN
            },
            el: {
                common: commonEL,
                auth: authEL,
                games: gamesEL,
                account: accountEL,
                profile: profileEL,
            },
        },
        lng: localStorage.getItem("lang") || "en",
        fallbackLng: "en",
            defaultNS: "common",
            ns: ["common", "auth", "games", "account", "profile"],
            interpolation: {
                escapeValue: false,
            },
        });


export default i18n;