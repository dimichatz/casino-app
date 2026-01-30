import {Card, CardContent} from "@/components/ui/card.tsx";
import {useTranslation} from "react-i18next";
import ChangePasswordForm from "@/components/account/profile/security/ChangePasswordForm.tsx";
import {useDocumentTitle} from "@/hooks/useDocumentTitle.ts";

const Security = () => {
    const { t } = useTranslation("profile");
    useDocumentTitle(t("common:documentTitle.security"));

    return (
        <>
            <div className="flex flex-col text-casinoapp-black w-full max-w-sm">
                <span className="text-2xl font-bold text-white p-6">
                    {t("security.title")}
                </span>

                <Card className="w-full rounded-2xl shadow-sm shadow-casinoapp-light-gray">
                    <CardContent className="p-10 pb-6">

                        <div>
                            <ChangePasswordForm/>
                        </div>

                    </CardContent>
                </Card>
            </div>
        </>
    );
};

export default Security;
