import { clsx, type ClassValue } from "clsx";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export function formatBytes(bytes: number, decimals = 1): string {
  if (!Number.isFinite(bytes) || bytes < 0) return "0 B";
  if (bytes === 0) return "0 B";

  const units = ["B", "KB", "MB", "GB", "TB"];
  const k = 1024;
  const i = Math.min(
    Math.floor(Math.log(bytes) / Math.log(k)),
    units.length - 1,
  );
  const value = bytes / Math.pow(k, i);
  const fixed = value.toFixed(decimals).replace(".", ",");

  return `${fixed} ${units[i]}`;
}

export function formatDuration(seconds: number | null): string {
  if (seconds === null || !Number.isFinite(seconds) || seconds < 0) {
    return "—";
  }

  const total = Math.round(seconds);
  const h = Math.floor(total / 3600);
  const m = Math.floor((total % 3600) / 60);
  const s = total % 60;

  if (h > 0) {
    return `${h}:${String(m).padStart(2, "0")}:${String(s).padStart(2, "0")}`;
  }

  return `${m}:${String(s).padStart(2, "0")}`;
}

export function formatPercent(value: number, decimals = 1): string {
  return `${value.toFixed(decimals).replace(".", ",")}%`;
}

export function estimateCompressedSize(
  originalSize: number,
  reductionRatio: number,
): number {
  return Math.max(Math.round(originalSize * (1 - reductionRatio)), 1);
}
