import { useTranslation } from "react-i18next";
import eeepLogo from "@/assets/images/eeep-large-en.svg";
import ketheaLogo from "@/assets/images/kethea_logo.png";
import ageLogo from "@/assets/images/21.webp";

const Footer = () => {
    const { t } = useTranslation("common");
    const currentYear:number = new Date().getFullYear();

    return (
        <>
            <div className="bg-casinoapp-black w-full select-none">
                <div className=" mx-auto py-8 text-left">
                    <div className="mx-4">
                        <h6 className="text-lg font-semibold text-white mb-2">
                            {t("footer.about.title")}
                        </h6>
                        <p className="text-sm text-casinoapp-light-gray">
                            {t("footer.about.description")}
                        </p>
                    </div>
                    <hr className="border-t border-white my-6" />
                    <div className="mx-4 flex items-center gap-16">
                        <img src={eeepLogo} alt="EEEP Logo"/>
                        <h1 className="text-white text-2xl">
                            {t("footer.responsibleGaming")}
                        </h1>
                    </div>
                    <hr className="border-t border-white my-6" />
                    <div className="mx-4 flex items-center gap-16">
                        <img className="invert brightness-0 pb-1" src={ageLogo} alt="21+"/>
                        <img className="h-16" src={ketheaLogo} alt="KETHEA Logo"/>
                    </div>
                    <div className="mx-auto py-8 text-center text-casinoapp-light-gray">
                        &copy; {currentYear} {t("footer.copyright")}
                    </div>
                    <div className="mx-auto text-center">
                        <i className="fa-brands fa-square-facebook text-2xl text-casinoapp-light-gray"></i>
                        <i className="fa-brands fa-square-x-twitter text-2xl text-casinoapp-light-gray"></i>
                        <i className="fa-brands fa-square-linkedin text-2xl text-casinoapp-light-gray"></i>
                    </div>
                </div>
            </div>
        </>
    );
};

export default Footer;