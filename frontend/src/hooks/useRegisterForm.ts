import { useForm, type UseFormReturn } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {registerSchema} from "@/schemas/register";
import type {RegisterFields} from "@/schemas/register";

export const useRegisterForm = (): UseFormReturn<RegisterFields> => {
    return useForm<RegisterFields>({
        resolver: zodResolver(registerSchema),
        mode: "onChange",
        reValidateMode: "onChange",
        delayError: 800,
        shouldUnregister: false,
        defaultValues: {
            birthDate: "",
            gender: "Male",
            isAgeVerified: false,
            hasAcceptedTerms: false,
        },
    });
};
