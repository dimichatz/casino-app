import Dropzone, {type FileRejection} from "react-dropzone";
import {useTranslation} from "react-i18next";
import {CirclePlus, CircleX, Trash2 } from "lucide-react";
import {useState} from "react";

type FileDropzoneProps = {
    file: File | null;
    onFileSelect: (file: File) => void;
    onFileRemove: () => void;
    disabled?: boolean;
};

const FileDropzone = ({
                          file,
                          onFileSelect,
                          onFileRemove,
                          disabled,
                      }: FileDropzoneProps) => {
    const { t } = useTranslation("profile");
    const [error, setError] = useState<string | null>(null);

    const handleRejected = (rejections: FileRejection[]) => {
        const rejection = rejections[0];
        const err = rejection?.errors[0];

        if (!err) return;

        switch (err.code) {
            case "file-too-large":
                setError(t("verification.upload.errors.tooLarge"));
                break;
            case "file-too-small":
                setError(t("verification.upload.errors.tooSmall"));
                break;
            case "file-invalid-type":
                setError(t("verification.upload.errors.invalidType"));
                break;
            default:
                setError(t("verification.upload.errors.generic"));
        }
    };

    return (
        <div className="space-y-2">
            <Dropzone
                onDrop={(files) => {
                    setError(null);
                    onFileSelect(files[0]);
                }}
                onDropRejected={handleRejected}
                multiple={false}
                disabled={disabled || !!file}
                maxSize={5 * 1024 * 1024}
                minSize={1024}
                accept={{
                    "image/jpeg": [],
                    "image/png": [],
                    "application/pdf": [],
                }}
            >
                {({ getRootProps, getInputProps, isDragActive }) => (
                    <div
                        {...getRootProps()}
                        className={`rounded-lg border-2 border-dashed transition
                        ${isDragActive ? "bg-zinc-50 border-zinc-500" : ""}
                    `}
                    >
                        <input {...getInputProps()} />

                        {file ? (
                            <div className="flex w-full items-center justify-center p-8">
                                <div className="flex w-auto p-2 bg-zinc-100 gap-2 rounded-lg">
                                    <div className="text-sm truncate cursor-default">
                                        {file.name}
                                    </div>

                                    {!disabled && (
                                        <button
                                            type="button"
                                            onClick={(e) => {
                                                e.stopPropagation();
                                                onFileRemove();
                                            }}
                                            className="cursor-pointer"
                                        >
                                            <Trash2  className="h-6 w-6" />
                                        </button>
                                    )}
                                </div>
                                </div>

                        ) : (
                            <div
                                className="w-full p-8 flex flex-col
                                items-center text-center cursor-pointer"
                            >
                                <CirclePlus className="w-10 h-10 text-casinoapp-black/80" />
                                <p className="text-xs font-base mt-1">
                                    {t("verification.upload.dragOrDrop")}
                                </p>
                                <p className="text-xs text-muted-foreground mt-0.5">
                                    {t("verification.upload.fileRestrictions")}
                                </p>
                            </div>
                        )}
                    </div>
                )}
            </Dropzone>

            {error && (
                <div className="flex items-center gap-1 mt-2 text-xs text-red-500">
                    <CircleX className="h-4 w-4" />
                    <span>
                            {error}
                        </span>
                </div>
            )}

        </div>
    );


};
export default FileDropzone;