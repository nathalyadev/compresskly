import { ERROR_MESSAGES } from "@/lib/constants";
import type {
  ApiProgressEvent,
  CompressionQuality,
  CompressionResult,
} from "@/types/video";

const API_BASE_URL =
  process.env.NEXT_PUBLIC_API_URL?.replace(/\/$/, "") || "http://localhost:5080";

function mapApiStatus(status?: string): ApiProgressEvent["status"] {
  switch ((status || "").toLowerCase()) {
    case "preparing":
      return "preparing";
    case "processing":
    case "queued":
    case "uploaded":
      return "processing";
    case "compressing":
      return "compressing";
    case "finalizing":
      return "finalizing";
    case "completed":
      return "completed";
    case "failed":
      return "error";
    default:
      return "processing";
  }
}

async function readErrorMessage(response: Response): Promise<string> {
  try {
    const data = (await response.json()) as { message?: string };
    if (data.message) return data.message;
  } catch {
    // ignore parse errors
  }
  return ERROR_MESSAGES.processingFailed;
}

export async function compressVideoRequest(params: {
  file: File;
  quality: CompressionQuality;
  signal?: AbortSignal;
  onEvent: (event: ApiProgressEvent) => void;
}): Promise<CompressionResult> {
  const { file, quality, signal, onEvent } = params;

  onEvent({
    type: "status",
    status: "preparing",
    progress: 2,
    message: "Preparando vídeo...",
  });

  const formData = new FormData();
  formData.append("video", file);

  let uploadResponse: Response;
  try {
    uploadResponse = await fetch(`${API_BASE_URL}/api/videos/upload`, {
      method: "POST",
      body: formData,
      signal,
    });
  } catch (error) {
    if (error instanceof DOMException && error.name === "AbortError") {
      throw new Error(ERROR_MESSAGES.uploadInterrupted);
    }
    throw new Error(ERROR_MESSAGES.uploadInterrupted);
  }

  if (!uploadResponse.ok) {
    throw new Error(await readErrorMessage(uploadResponse));
  }

  const uploaded = (await uploadResponse.json()) as {
    id: string;
    originalFileName: string;
    originalSize: number;
  };

  onEvent({
    type: "status",
    status: "processing",
    progress: 10,
    message: "Processando...",
  });

  let compressResponse: Response;
  try {
    compressResponse = await fetch(
      `${API_BASE_URL}/api/videos/${uploaded.id}/compress`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ quality }),
        signal,
      },
    );
  } catch (error) {
    if (error instanceof DOMException && error.name === "AbortError") {
      throw new Error(ERROR_MESSAGES.uploadInterrupted);
    }
    throw new Error(ERROR_MESSAGES.processingFailed);
  }

  if (!compressResponse.ok && compressResponse.status !== 202) {
    throw new Error(await readErrorMessage(compressResponse));
  }

  onEvent({
    type: "status",
    status: "compressing",
    progress: 15,
    message: "Comprimindo...",
  });

  const pollIntervalMs = 1000;
  const maxAttempts = 60 * 30; // ~30 min

  for (let attempt = 0; attempt < maxAttempts; attempt++) {
    if (signal?.aborted) {
      throw new Error(ERROR_MESSAGES.uploadInterrupted);
    }

    const statusResponse = await fetch(
      `${API_BASE_URL}/api/videos/${uploaded.id}/status`,
      { signal },
    );

    if (!statusResponse.ok) {
      throw new Error(await readErrorMessage(statusResponse));
    }

    const status = (await statusResponse.json()) as {
      id: string;
      status: string;
      progress: number;
      message?: string;
      originalSize: number;
      compressedSize?: number | null;
      savedBytes?: number | null;
      reductionPercent?: number | null;
      downloadUrl?: string | null;
      fileName?: string | null;
      errorMessage?: string | null;
    };

    const mappedStatus = mapApiStatus(status.status);

    if (status.status.toLowerCase() === "failed") {
      onEvent({
        type: "error",
        status: "error",
        message: status.errorMessage || ERROR_MESSAGES.processingFailed,
      });
      throw new Error(status.errorMessage || ERROR_MESSAGES.processingFailed);
    }

    if (status.status.toLowerCase() === "completed") {
      if (
        status.compressedSize == null ||
        !status.downloadUrl ||
        !status.fileName
      ) {
        throw new Error(ERROR_MESSAGES.outputFailed);
      }

      const downloadUrl = status.downloadUrl.startsWith("http")
        ? status.downloadUrl
        : `${API_BASE_URL}${status.downloadUrl}`;

      const result: CompressionResult = {
        jobId: status.id,
        originalSize: status.originalSize,
        compressedSize: status.compressedSize,
        savedBytes:
          status.savedBytes ?? status.originalSize - status.compressedSize,
        reductionPercent:
          status.reductionPercent ??
          ((status.originalSize - status.compressedSize) /
            status.originalSize) *
            100,
        downloadUrl,
        fileName: status.fileName,
      };

      onEvent({
        type: "complete",
        status: "completed",
        progress: 100,
        message: status.message || "Compressão concluída",
        jobId: result.jobId,
        originalSize: result.originalSize,
        compressedSize: result.compressedSize,
        savedBytes: result.savedBytes,
        reductionPercent: result.reductionPercent,
        downloadUrl: result.downloadUrl,
        fileName: result.fileName,
      });

      return result;
    }

    onEvent({
      type: "progress",
      status: mappedStatus,
      progress: status.progress,
      message: status.message || "Comprimindo...",
    });

    await new Promise((resolve) => setTimeout(resolve, pollIntervalMs));
  }

  throw new Error(ERROR_MESSAGES.processingFailed);
}

export async function downloadCompressedVideo(params: {
  downloadUrl: string;
  fileName: string;
}): Promise<void> {
  const response = await fetch(params.downloadUrl);

  if (!response.ok) {
    throw new Error(ERROR_MESSAGES.downloadFailed);
  }

  const blob = await response.blob();
  const objectUrl = URL.createObjectURL(blob);
  const anchor = document.createElement("a");
  anchor.href = objectUrl;
  anchor.download = params.fileName;
  document.body.appendChild(anchor);
  anchor.click();
  anchor.remove();
  URL.revokeObjectURL(objectUrl);
}
