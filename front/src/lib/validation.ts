import {
  ACCEPTED_EXTENSION,
  ACCEPTED_MIME_TYPES,
  ERROR_MESSAGES,
  MAX_FILE_SIZE_BYTES,
} from "@/lib/constants";

export interface ValidationResult {
  valid: boolean;
  error?: string;
  code?: "invalid" | "too_large";
}

export function validateClientFile(file: File): ValidationResult {
  const lowerName = file.name.toLowerCase();

  if (!lowerName.endsWith(ACCEPTED_EXTENSION)) {
    return { valid: false, error: ERROR_MESSAGES.notMp4, code: "invalid" };
  }

  if (
    file.type &&
    !ACCEPTED_MIME_TYPES.includes(
      file.type as (typeof ACCEPTED_MIME_TYPES)[number],
    )
  ) {
    return { valid: false, error: ERROR_MESSAGES.notMp4, code: "invalid" };
  }

  if (file.size > MAX_FILE_SIZE_BYTES) {
    return { valid: false, error: ERROR_MESSAGES.tooLarge, code: "too_large" };
  }

  if (file.size === 0) {
    return { valid: false, error: ERROR_MESSAGES.notMp4, code: "invalid" };
  }

  return { valid: true };
}

export function sanitizeFileName(originalName: string): string {
  const base = originalName
    .replace(/[^a-zA-Z0-9._-]/g, "_")
    .replace(/_{2,}/g, "_")
    .slice(0, 120);

  const withoutExt = base.toLowerCase().endsWith(".mp4")
    ? base.slice(0, -4)
    : base;

  const safe = withoutExt || "video";
  return `${safe}.mp4`;
}

export function buildCompressedFileName(originalName: string): string {
  const safe = sanitizeFileName(originalName);
  const stem = safe.replace(/\.mp4$/i, "");
  return `${stem}-comprimido.mp4`;
}
