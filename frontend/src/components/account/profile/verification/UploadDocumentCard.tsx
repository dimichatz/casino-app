import CollapsibleCard from "@/components/account/profile/verification/CollapsibleCard.tsx";
import {useTranslation} from "react-i18next";
import {FileCheck, FileUp} from "lucide-react";
import {Button} from "@/components/ui/button.tsx";
import {useState} from "react";
import FileDropzone from "@/components/account/profile/verification/FileDropzone.tsx";
import {uploadKycDocument} from "@/services/api.player.ts";
import {toast} from "sonner";

const UploadDocumentCard = () => {
    const { t } = useTranslation("profile");
    const [file, setFile] = useState<File | null>(null);
    const [isUploading, setIsUploading] = useState(false);

    const handleUpload = async () => {
        if (!file || isUploading) return;

        setIsUploading(true);
        try {
            await uploadKycDocument(file);
            toast.success(t("verification.upload.success"));
            setFile(null);
        } catch (err: unknown) {
            console.warn("Request rejected:", err);
            toast.error(t("verification.upload.errors.generic2"));
        } finally {
            setIsUploading(false);
        }
    };

    return(
        <CollapsibleCard
            title={
                <div className="flex gap-2">
                    <FileCheck />
                    {t("verification.upload.title")}
                </div>
            }
        >
            <div className="space-y-4">

                <FileDropzone
                    file={file}
                    onFileSelect={setFile}
                    onFileRemove={() => setFile(null)}
                />

                <Button
                    onClick={handleUpload}
                    disabled={!file || isUploading}
                    className="text-zinc-200 bg-casinoapp-black/95
                        hover:bg-casinoapp-black hover:text-white cursor-pointer"
                    type="button"
                >
                    <div className="flex gap-1 items-center">
                        <FileUp />
                        {t("verification.upload.uploadBtn")}
                    </div>
                </Button>
            </div>
        </CollapsibleCard>
    );
};

export default UploadDocumentCard;