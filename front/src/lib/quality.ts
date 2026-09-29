import { QUALITY_OPTIONS } from "@/lib/constants";
import type { CompressionQuality } from "@/types/video";

export function isValidQuality(value: unknown): value is CompressionQuality {
  return (
    typeof value === "string" &&
    QUALITY_OPTIONS.some((option) => option.value === value)
  );
}
