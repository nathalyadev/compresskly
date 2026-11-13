"use client";

import { useCallback, useEffect, useRef, useState } from "react";

import { CompressionProgressPanel } from "@/components/CompressionProgressPanel";
import { CompressionResultCard } from "@/components/CompressionResultCard";
import { ErrorAlert } from "@/components/ErrorAlert";
import { QualitySelector } from "@/components/QualitySelector";
import { VideoPreviewCard } from "@/components/VideoPreviewCard";
import { VideoUploadZone } from "@/components/VideoUploadZone";
import { useVideoDuration } from "@/hooks/use-video-duration";
import {
  compressVideoRequest,
  downloadCompressedVideo,
} from "@/lib/api-client";
import { ERROR_MESSAGES, STATUS_MESSAGES } from "@/lib/constants";
import type {
  AppState,
  CompressionProgress,
  CompressionQuality,
  CompressionResult,
  VideoFileInfo,
} from "@/types/video";
import { Button } from "@/components/ui/components";

export function VideoCompressor() {
  const [appState, setAppState] = useState<AppState>("idle");
  const [selected, setSelected] = useState<VideoFileInfo | null>(null);
  const [quality, setQuality] = useState<CompressionQuality>("balanced");
  const [progress, setProgress] = useState<CompressionProgress>({
    progress: 0,
    status: "preparing",
    message: STATUS_MESSAGES.preparing,
  });
  const [result, setResult] = useState<CompressionResult | null>(null);
  const [resultPreviewUrl, setResultPreviewUrl] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [downloading, setDownloading] = useState(false);

  const abortRef = useRef<AbortController | null>(null);
  const duration = useVideoDuration(selected?.previewUrl ?? null);

  useEffect(() => {
    return () => {
      abortRef.current?.abort();
      if (selected?.previewUrl) URL.revokeObjectURL(selected.previewUrl);
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const clearSelectedPreview = useCallback(() => {
    if (selected?.previewUrl) {
      URL.revokeObjectURL(selected.previewUrl);
    }
  }, [selected]);

  const clearResultPreview = useCallback(() => {
    setResultPreviewUrl(null);
  }, []);

  const resetAll = useCallback(() => {
    abortRef.current?.abort();
    abortRef.current = null;
    clearSelectedPreview();
    clearResultPreview();
    setSelected(null);
    setResult(null);
    setErrorMessage(null);
    setQuality("balanced");
    setProgress({
      progress: 0,
      status: "preparing",
      message: STATUS_MESSAGES.preparing,
    });
    setAppState("idle");
    setDownloading(false);
  }, [clearResultPreview, clearSelectedPreview]);

  const handleFileAccepted = useCallback(
    (file: File) => {
      clearSelectedPreview();
      clearResultPreview();
      setResult(null);
      setErrorMessage(null);

      const previewUrl = URL.createObjectURL(file);
      setSelected({
        file,
        name: file.name,
        size: file.size,
        duration: null,
        previewUrl,
      });
      setAppState("selected");
    },
    [clearResultPreview, clearSelectedPreview],
  );

  const handleValidationError = useCallback(
    (message: string, code: "invalid" | "too_large") => {
      setErrorMessage(message);
      setAppState(code === "too_large" ? "too_large" : "invalid");
    },
    [],
  );

  const handleCompress = useCallback(async () => {
    if (!selected) return;

    abortRef.current?.abort();
    const controller = new AbortController();
    abortRef.current = controller;

    setErrorMessage(null);
    setResult(null);
    clearResultPreview();
    setAppState("preparing");
    setProgress({
      progress: 2,
      status: "preparing",
      message: STATUS_MESSAGES.preparing,
    });

    try {
      const compressionResult = await compressVideoRequest({
        file: selected.file,
        quality,
        signal: controller.signal,
        onEvent: (event) => {
          if (event.type === "status" || event.type === "progress") {
            setAppState("processing");
            setProgress({
              progress: event.progress ?? 0,
              status: event.status || "processing",
              message:
                event.message ||
                STATUS_MESSAGES[event.status || "processing"] ||
                STATUS_MESSAGES.processing,
            });
          }
        },
      });

      setResultPreviewUrl(`${compressionResult.downloadUrl}?preview=1`);
      setResult(compressionResult);
      setAppState("completed");
      setProgress({
        progress: 100,
        status: "completed",
        message: STATUS_MESSAGES.completed,
      });
    } catch (error) {
      const message =
        error instanceof Error ? error.message : ERROR_MESSAGES.generic;
      setErrorMessage(message);
      setAppState("error");
    }
  }, [clearResultPreview, quality, selected]);

  const handleDownload = useCallback(async () => {
    if (!result) return;

    setDownloading(true);
    setErrorMessage(null);

    try {
      await downloadCompressedVideo({
        downloadUrl: result.downloadUrl,
        fileName: result.fileName,
      });
    } catch {
      setErrorMessage(ERROR_MESSAGES.downloadFailed);
    } finally {
      setDownloading(false);
    }
  }, [result]);

  const isBusy = appState === "preparing" || appState === "processing";
  const selectedWithDuration = selected
    ? { ...selected, duration: duration ?? selected.duration }
    : null;

  return (
    <div className="mx-auto flex w-full max-w-3xl flex-col gap-6 px-4 py-8 sm:px-6 sm:py-10">
      <div className="space-y-2">
        <h1 className="text-3xl font-normal tracking-tight text-[#202124] sm:text-4xl">
          Comprima seus vídeos
        </h1>
        <p className="max-w-2xl text-base text-[#5f6368] sm:text-lg">
          Reduza o tamanho de arquivos MP4 mantendo uma boa qualidade visual.
          Ideal para compartilhar, enviar e armazenar com mais facilidade.
        </p>
      </div>

      {errorMessage ? (
        <ErrorAlert
          message={errorMessage}
          onDismiss={() => {
            setErrorMessage(null);
            if (appState === "invalid" || appState === "too_large") {
              setAppState(selected ? "selected" : "idle");
            }
          }}
        />
      ) : null}

      {appState === "idle" ||
      appState === "invalid" ||
      appState === "too_large" ? (
        <VideoUploadZone
          onFileAccepted={handleFileAccepted}
          onValidationError={handleValidationError}
        />
      ) : null}

      {(appState === "selected" || appState === "error") &&
      selectedWithDuration ? (
        <div className="flex flex-col gap-5">
          <VideoPreviewCard
            name={selectedWithDuration.name}
            size={selectedWithDuration.size}
            duration={selectedWithDuration.duration}
            previewUrl={selectedWithDuration.previewUrl}
            label="Pré-visualização do vídeo original"
          />

          <QualitySelector
            value={quality}
            originalSize={selectedWithDuration.size}
            onChange={setQuality}
            disabled={isBusy}
          />

          <div className="flex flex-col gap-3 sm:flex-row">
            <Button
              type="button"
              size="lg"
              className="w-full sm:flex-1"
              onClick={handleCompress}
              disabled={isBusy}
            >
              Iniciar compressão
            </Button>
            <Button
              type="button"
              variant="outline"
              size="lg"
              className="w-full sm:flex-1"
              onClick={resetAll}
              disabled={isBusy}
            >
              Escolher outro arquivo
            </Button>
          </div>
        </div>
      ) : null}

      {isBusy ? <CompressionProgressPanel progress={progress} /> : null}

      {appState === "completed" && result && resultPreviewUrl ? (
        <CompressionResultCard
          result={result}
          previewUrl={resultPreviewUrl}
          onDownload={handleDownload}
          onReset={resetAll}
          downloading={downloading}
        />
      ) : null}
    </div>
  );
}
