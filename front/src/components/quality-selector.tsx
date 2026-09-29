"use client";

import { QUALITY_OPTIONS } from "@/lib/constants";
import { estimateCompressedSize, formatBytes, formatPercent } from "@/lib/utils";
import type { CompressionQuality } from "@/types/video";
import { Label } from "@/components/ui/label";
import { cn } from "@/lib/utils";

interface QualitySelectorProps {
  value: CompressionQuality;
  originalSize: number;
  disabled?: boolean;
  onChange: (value: CompressionQuality) => void;
}

export function QualitySelector({
  value,
  originalSize,
  disabled = false,
  onChange,
}: QualitySelectorProps) {
  const selected = QUALITY_OPTIONS.find((option) => option.value === value);
  const estimate = selected
    ? estimateCompressedSize(originalSize, selected.estimatedReduction)
    : originalSize;
  const reduction = selected ? selected.estimatedReduction * 100 : 0;

  return (
    <fieldset className="w-full" disabled={disabled}>
      <legend className="mb-3 text-sm font-medium text-[#202124]">
        Qualidade da compressão
      </legend>
      <div className="grid gap-2" role="radiogroup" aria-label="Qualidade">
        {QUALITY_OPTIONS.map((option) => {
          const checked = option.value === value;
          return (
            <label
              key={option.value}
              className={cn(
                "flex cursor-pointer items-start gap-3 rounded-xl border px-4 py-3 transition-colors",
                checked
                  ? "border-[#1a73e8] bg-[#e8f0fe]"
                  : "border-[#dadce0] bg-white hover:bg-[#f8f9fa]",
                disabled && "cursor-not-allowed opacity-60",
              )}
            >
              <input
                type="radio"
                name="quality"
                value={option.value}
                checked={checked}
                disabled={disabled}
                onChange={() => onChange(option.value)}
                className="mt-1 h-4 w-4 accent-[#1a73e8]"
                aria-describedby={`quality-${option.value}-desc`}
              />
              <span className="min-w-0">
                <Label className="cursor-pointer">{option.label}</Label>
                <span
                  id={`quality-${option.value}-desc`}
                  className="mt-0.5 block text-sm text-[#5f6368]"
                >
                  {option.description}
                </span>
              </span>
            </label>
          );
        })}
      </div>
      <p className="mt-3 text-sm text-[#5f6368]" aria-live="polite">
        Estimativa aproximada:{" "}
        <span className="font-medium text-[#202124]">
          {formatBytes(estimate)}
        </span>{" "}
        ({formatPercent(reduction)} de redução)
      </p>
    </fieldset>
  );
}
