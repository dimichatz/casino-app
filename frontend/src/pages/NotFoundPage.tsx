import { Link } from "react-router-dom";
import {useTranslation} from "react-i18next";

const NotFoundPage = () => {
    const { t } = useTranslation("common");

    return (
        <div className="min-h-screen flex flex-col items-center justify-center text-center px-4">
            <h1 className="text-6xl font-bold text-white mb-4">
                {t("404.title")}
            </h1>

            <p className="text-lg text-casinoapp-light-gray mb-6">
                {t("404.description")}
            </p>

            <Link
                to="/"
                className="px-6 py-3 rounded-lg text-white bg-casinoapp-dark-blue hover:opacity-90 transition"
            >
                {t("404.return")}
            </Link>
        </div>
    );
};

export default NotFoundPage;
