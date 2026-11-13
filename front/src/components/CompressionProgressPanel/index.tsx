"use client";

import type { CompressionProgress } from "@/types/video";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
  Progress,
} from "@/components/ui/components";

interface CompressionProgressPanelProps {
  progress: CompressionProgress;
}

export function CompressionProgressPanel({
  progress,
}: CompressionProgressPanelProps) {
  const value = Math.max(0, Math.min(100, progress.progress));

  return (
    <Card aria-live="polite" aria-busy="true">
      <CardHeader className="border-b">
        <div className="flex items-center justify-between gap-3">
          <CardTitle>{progress.message}</CardTitle>
          <p className="text-sm font-semibold tabular-nums text-primary">
            {value}%
          </p>
        </div>
        <CardDescription>
          Isso pode levar alguns minutos, dependendo do tamanho do vídeo.
        </CardDescription>
      </CardHeader>
      <CardContent>
        <Progress
          value={value}
          aria-label={`Progresso da compressão: ${value}%`}
        />
      </CardContent>
    </Card>
  );
}
