export type CompressionQuality = "high" | "balanced" | "max";

export type AppState =
  | "idle"
  | "selected"
  | "preparing"
  | "processing"
  | "completed"
  | "error"
  | "invalid"
  | "too_large";

export type CompressionStatus =
  | "preparing"
  | "processing"
  | "compressing"
  | "finalizing"
  | "completed"
  | "error";

export interface VideoFileInfo {
  file: File;
  name: string;
  size: number;
  duration: number | null;
  previewUrl: string;
}

export interface CompressionProgress {
  progress: number;
  status: CompressionStatus;
  message: string;
}

export interface CompressionResult {
  jobId: string;
  originalSize: number;
  compressedSize: number;
  savedBytes: number;
  reductionPercent: number;
  downloadUrl: string;
  fileName: string;
  previewUrl?: string;
}

export interface CompressionEstimate {
  estimatedSize: number;
  estimatedReductionPercent: number;
}

export interface ApiProgressEvent {
  type: "status" | "progress" | "complete" | "error";
  progress?: number;
  message?: string;
  status?: CompressionStatus;
  jobId?: string;
  originalSize?: number;
  compressedSize?: number;
  savedBytes?: number;
  reductionPercent?: number;
  downloadUrl?: string;
  fileName?: string;
}
