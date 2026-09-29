"use client";

import { useCallback, useId, useRef, useState } from "react";
import { Upload } from "lucide-react";
import { Button } from "@/components/ui/button";
import { ACCEPTED_EXTENSION, ERROR_MESSAGES } from "@/lib/constants";
import { validateClientFile } from "@/lib/validation";
import { cn } from "@/lib/utils";

interface VideoUploadZoneProps {
  disabled?: boolean;
  onFileAccepted: (file: File) => void;
  onValidationError: (message: string, code: "invalid" | "too_large") => void;
}

export function VideoUploadZone({
  disabled = false,
  onFileAccepted,
  onValidationError,
}: VideoUploadZoneProps) {
  const inputId = useId();
  const inputRef = useRef<HTMLInputElement>(null);
  const [isDragging, setIsDragging] = useState(false);

  const handleFile = useCallback(
    (file: File | undefined | null) => {
      if (!file) return;

      const result = validateClientFile(file);
      if (!result.valid) {
        onValidationError(
          result.error || ERROR_MESSAGES.notMp4,
          result.code || "invalid",
        );
        return;
      }

      onFileAccepted(file);
    },
    [onFileAccepted, onValidationError],
  );

  return (
    <div
      role="button"
      tabIndex={disabled ? -1 : 0}
      aria-disabled={disabled}
      aria-label="Área para enviar vídeo MP4. Arraste um arquivo ou pressione Enter para selecionar."
      className={cn(
        "flex w-full cursor-pointer flex-col items-center justify-center rounded-2xl border-2 border-dashed px-4 py-12 text-center transition-colors outline-none focus-visible:ring-3 focus-visible:ring-ring/50 sm:py-16",
        isDragging
          ? "border-primary bg-accent"
          : "border-border bg-muted/40 hover:border-primary/60 hover:bg-muted/70",
        disabled && "pointer-events-none opacity-60",
      )}
      onKeyDown={(event) => {
        if (disabled) return;
        if (event.key === "Enter" || event.key === " ") {
          event.preventDefault();
          inputRef.current?.click();
        }
      }}
      onClick={() => {
        if (!disabled) inputRef.current?.click();
      }}
      onDragEnter={(event) => {
        event.preventDefault();
        event.stopPropagation();
        if (!disabled) setIsDragging(true);
      }}
      onDragOver={(event) => {
        event.preventDefault();
        event.stopPropagation();
        if (!disabled) setIsDragging(true);
      }}
      onDragLeave={(event) => {
        event.preventDefault();
        event.stopPropagation();
        setIsDragging(false);
      }}
      onDrop={(event) => {
        event.preventDefault();
        event.stopPropagation();
        setIsDragging(false);
        if (disabled) return;
        handleFile(event.dataTransfer.files?.[0]);
      }}
    >
      <div className="mb-4 flex h-12 w-12 items-center justify-center rounded-full bg-card shadow-sm ring-1 ring-border">
        <Upload className="h-5 w-5 text-primary" aria-hidden />
      </div>
      <p className="text-base font-medium text-foreground">
        Arraste seu vídeo MP4 aqui
      </p>
      <p className="mt-1 max-w-sm text-sm text-muted-foreground">
        Ou selecione um arquivo do seu dispositivo. Apenas vídeos .mp4 são
        aceitos.
      </p>
      <Button
        type="button"
        size="lg"
        className="mt-6"
        disabled={disabled}
        onClick={(event) => {
          event.stopPropagation();
          inputRef.current?.click();
        }}
      >
        Selecionar arquivo
      </Button>
      <input
        ref={inputRef}
        id={inputId}
        type="file"
        accept={`${ACCEPTED_EXTENSION},video/mp4`}
        className="sr-only"
        disabled={disabled}
        aria-label="Selecionar arquivo de vídeo MP4"
        onChange={(event) => {
          handleFile(event.target.files?.[0]);
          event.target.value = "";
        }}
      />
    </div>
  );
}
