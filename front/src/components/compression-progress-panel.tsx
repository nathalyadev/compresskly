"use client";

import { Progress } from "@/components/ui/progress";
import type { CompressionProgress } from "@/types/video";

interface CompressionProgressPanelProps {
  progress: CompressionProgress;
}

export function CompressionProgressPanel({
  progress,
}: CompressionProgressPanelProps) {
  const value = Math.max(0, Math.min(100, progress.progress));

  return (
    <section
      className="w-full rounded-2xl border border-[#e8eaed] bg-white px-4 py-6 sm:px-6"
      aria-live="polite"
      aria-busy="true"
    >
      <div className="mb-4 flex items-center justify-between gap-3">
        <h2 className="text-base font-medium text-[#202124]">
          {progress.message}
        </h2>
        <p className="text-sm font-semibold tabular-nums text-[#1a73e8]">
          {value}%
        </p>
      </div>
      <Progress value={value} aria-label={`Progresso da compressão: ${value}%`} />
      <p className="mt-3 text-sm text-[#5f6368]">
        Isso pode levar alguns minutos, dependendo do tamanho do vídeo.
      </p>
    </section>
  );
}
