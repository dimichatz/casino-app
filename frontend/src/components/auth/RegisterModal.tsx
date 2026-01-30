import { useTranslation } from "react-i18next";
import {useUI} from "@/hooks/useUI.ts";
import {X, ArrowLeft } from "lucide-react";
import logo from "@/assets/images/promo10.png";
import {type RegisterFields, registerSubmitSchema, step1Schema, step2Schema, step3Schema} from "@/schemas/register";
import {useRegisterForm} from "@/hooks/useRegisterForm.ts";
import {useContext, useState} from "react";
import {Step1Account} from "@/components/auth/steps/Step1Account.tsx";
import {Step2Identity} from "@/components/auth/steps/Step2Identity.tsx";
import {Step3Address} from "@/components/auth/steps/Step3Address.tsx";
import type {FieldPath} from "react-hook-form";
import {registerPlayer} from "@/services/api.register.ts";
import { useNavigate } from "react-router-dom";
import {AuthContext} from "@/context/AuthContext.ts";
import {toast} from "sonner";

const RegisterModal = () => {
    const { activeModal, openLogin, closeModal } = useUI();
    const { t } = useTranslation("auth");
    const navigate = useNavigate();
    const auth = useContext(AuthContext);

    if (!auth) {
        throw new Error("AuthContext must be used within AuthProvider");
    }

    const { loginUser } = auth;
    const form = useRegisterForm();
    const [step, setStep] = useState<1 | 2 | 3>(1);

    const stepSchemas = {
        1: step1Schema,
        2: step2Schema,
        3: step3Schema,
    };

    const stepFields: Record<1 | 2 | 3, (keyof RegisterFields)[]> = {
        1: ["email", "username", "password", "confirmPassword"],
        2: ["phoneNumber", "firstname", "lastname", "gender", "birthDate"],
        3: ["documentType", "documentNumber", "streetName", "streetNumber",
            "postalCode", "city", "isAgeVerified", "hasAcceptedTerms"],
    };

    const {
        formState: { isSubmitting, isValid }
    } = form;

    const onSubmit = async (data: RegisterFields) => {
        const payload = registerSubmitSchema.parse(data);

        try {
            await registerPlayer(payload);
            await loginUser({
                usernameOrEmail: data.email,
                password: data.password,
            });

            toast.success(t("register.success"));
            form.reset();
            setStep(1);
            closeModal();
            navigate("/");
        } catch (error) {
            console.error("Signup / login failed:", error);
        }
    };

        const handleNext = () => {
            const values = form.getValues();
            const result = stepSchemas[step].safeParse(values);

            if (!result.success) {
                result.error.issues.forEach((issue) => {
                    const fieldName = issue.path.join(".") as FieldPath<RegisterFields>;
                    form.setError(fieldName, { message: issue.message });
                });
                return;
            }

            const currentStepFields = stepFields[step];

            const hasServerErrors = currentStepFields.some(
                (field) => form.formState.errors[field]
            );

            if (hasServerErrors) {
                return;
            }

            setStep((s) => (s === 1 ? 2 : 3));
        };

    const handlePrevious = () => {
        setStep(step === 3 ? 2 : 1);
    };

    const handleRegisterClick = async () => {
        const fields = stepFields[3];

        fields.forEach((field) => {
            form.setValue(field, form.getValues(field), {
                shouldTouch: true,
                shouldValidate: false,
            });
        });

        const isValid = await form.trigger(fields, {
            shouldFocus: true,
        });

        if (!isValid) return;

        await form.handleSubmit(onSubmit)();
    };

    if (activeModal != "register") return null;

    return (
        <>
            <div className="fixed inset-0 z-50 flex items-center justify-center">
                <div
                    className="absolute inset-0 bg-black/60"
                    onClick={closeModal}
                />

                <div
                    onClick={(e) => e.stopPropagation()}
                    className="relative w-full max-w-4xl -translate-y-10
                        flex rounded-lg bg-casinoapp-white h-[600px]"
                >

                    <div className="hidden md:block w-[43%]">
                        <img className="rounded-l-lg h-full w-auto object-cover object-left"
                             src={logo} alt="Promotion Logo"
                        />
                    </div>

                    <form
                        className="w-full md:w-[57%] p-4"
                         onSubmit={form.handleSubmit(onSubmit)}
                    >
                        <div
                            className="h-10 pb-2 border-b border-casinoapp-light-gray
                            flex items-center justify-end gap-1"
                        >
                            <p className="text-casinoapp-black">
                                {t("register.yesAccount")}
                            </p>
                            <button
                                type="button"
                                onClick={openLogin}
                                className=" cursor-pointer font-medium text-blue-700 hover:text-blue-600"
                            >
                                {t("register.login")}
                            </button>
                            <button
                                type="button"
                                onClick={closeModal}
                                className="ml-2 text-gray-400 hover:text-casinoapp-black"
                            >
                                <X aria-hidden="true" />
                            </button>
                        </div>

                        <div className="h-full">

                            <div className="h-10/12 flex justify-center mt-2">
                                <div className="overflow-y-auto">
                                    {step === 1 && <Step1Account form={form} />}
                                    {step === 2 && <Step2Identity form={form} />}
                                    {step === 3 && <Step3Address form={form} />}
                                </div>
                            </div>

                            <div
                                className="relative h-1/12 flex justify-between border-t-2
                                border-casinoapp-light-gray mx-4 pt-1"
                            >
                                <div
                                    className="absolute top-0 left-0 right-0 h-[4px]
                                     shadow-[0_-4px_8px_rgba(0,0,0,0.15)]"
                                />
                                <button
                                    type="button"
                                    onClick={handlePrevious} disabled={step === 1}
                                    className="text-center w-30 rounded-md text-casinoapp-black
                                    font-semibold cursor-pointer hover:opacity-85
                                    disabled:opacity-50 disabled:cursor-default"
                                >
                                    <div className="flex">
                                        <ArrowLeft className="h-6 w-6 " />
                                        <p>
                                            {t("register.previousButton")}
                                        </p>
                                    </div>

                                </button>
                                <button
                                    type={"button"}
                                    onClick={step === 3 ? handleRegisterClick : handleNext}
                                    disabled={isSubmitting || (step === 3 && !isValid)}
                                    className=" border bg-casinoapp-orange text-center
                                    w-30 rounded-md text-casinoapp-black font-semibold cursor-pointer
                                    hover:opacity-90 disabled:opacity-50 disabled:cursor-default"
                                >
                                    {step === 3
                                        ? t("register.registerButton")
                                        : t("register.nextButton")}
                                </button>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </>
    );
};

export default RegisterModal;