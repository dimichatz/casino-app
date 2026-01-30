import { X } from "lucide-react";
import LoginForm from "./LoginForm";
import {useUI} from "@/hooks/useUI.ts";
import { useTranslation } from "react-i18next";

const LoginModal = () => {
    const { activeModal, openRegister, closeModal } = useUI();
    const { t } = useTranslation("auth");

    if (activeModal != "login") return null;

    return (
        <>
            <div className="fixed inset-0 z-50 flex items-center justify-center">
                <div
                    className="absolute inset-0 bg-black/60"
                    onClick={closeModal}
                />

                <div
                    onClick={(e) => e.stopPropagation()}
                    className="relative w-full max-w-md sm:-translate-y-10 rounded-lg bg-casinoapp-white p-4">
                    <div
                        className="h-10 pb-2 border-b border-casinoapp-light-gray
                         flex items-center justify-end gap-1"
                    >
                        <p className="text-casinoapp-black">
                            {t("login.noAccount")}
                        </p>
                        <button
                            type="button"
                            onClick={openRegister}
                            className=" cursor-pointer font-medium text-blue-700 hover:text-blue-600"
                        >
                            {t("login.register")}
                        </button>
                        <button
                            onClick={closeModal}
                            className="ml-2 text-gray-400 hover:text-casinoapp-black"
                        >
                            <X aria-hidden="true" />
                        </button>
                    </div>
                    <div className="p-8 flex justify-center">
                        <LoginForm onSuccess={closeModal} />
                    </div>
                </div>
            </div>
        </>
    );
};

export default LoginModal;
