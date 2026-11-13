"use client";

import { QUALITY_OPTIONS } from "@/lib/constants";
import { estimateCompressedSize, formatBytes, formatPercent } from "@/lib/utils";
import { cn } from "@/lib/utils";
import type { CompressionQuality } from "@/types/video";
import { Label, RadioGroup, RadioGroupItem } from "@/components/ui/components";

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
      <legend className="mb-3 text-sm font-medium text-foreground">
        Qualidade da compressão
      </legend>
      <RadioGroup
        value={value}
        onValueChange={(next) => onChange(next as CompressionQuality)}
        className="grid gap-2"
        disabled={disabled}
        aria-label="Qualidade"
      >
        {QUALITY_OPTIONS.map((option) => {
          const checked = option.value === value;
          return (
            <label
              key={option.value}
              className={cn(
                "flex cursor-pointer items-start gap-3 rounded-xl border px-4 py-3 transition-colors",
                checked
                  ? "border-primary bg-accent"
                  : "border-border bg-card hover:bg-muted/60",
                disabled && "cursor-not-allowed opacity-60",
              )}
            >
              <RadioGroupItem
                value={option.value}
                id={`quality-${option.value}`}
                className="mt-1"
              />
              <span className="min-w-0">
                <Label
                  htmlFor={`quality-${option.value}`}
                  className="cursor-pointer"
                >
                  {option.label}
                </Label>
                <span
                  id={`quality-${option.value}-desc`}
                  className="mt-0.5 block text-sm text-muted-foreground"
                >
                  {option.description}
                </span>
              </span>
            </label>
          );
        })}
      </RadioGroup>
      <p className="mt-3 text-sm text-muted-foreground" aria-live="polite">
        Estimativa aproximada:{" "}
        <span className="font-medium text-foreground">
          {formatBytes(estimate)}
        </span>{" "}
        ({formatPercent(reduction)} de redução)
      </p>
    </fieldset>
  );
}
