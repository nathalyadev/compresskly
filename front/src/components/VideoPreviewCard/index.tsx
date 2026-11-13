"use client";

import { formatBytes, formatDuration } from "@/lib/utils";

interface VideoPreviewCardProps {
  name: string;
  size: number;
  duration: number | null;
  previewUrl: string;
  label?: string;
}

export function VideoPreviewCard({
  name,
  size,
  duration,
  previewUrl,
  label = "Pré-visualização",
}: VideoPreviewCardProps) {
  return (
    <div className="w-full overflow-hidden rounded-2xl border border-[#e8eaed] bg-white">
      <div className="border-b border-[#e8eaed] px-4 py-3 sm:px-5">
        <p className="truncate text-sm font-medium text-[#202124]" title={name}>
          {name}
        </p>
        <dl className="mt-2 flex flex-wrap gap-x-4 gap-y-1 text-sm text-[#5f6368]">
          <div className="flex gap-1">
            <dt>Tamanho:</dt>
            <dd className="font-medium text-[#202124]">{formatBytes(size)}</dd>
          </div>
          <div className="flex gap-1">
            <dt>Duração:</dt>
            <dd className="font-medium text-[#202124]">
              {formatDuration(duration)}
            </dd>
          </div>
        </dl>
      </div>
      <div className="bg-[#202124]/px-0">
        <video
          className="aspect-video w-full bg-black object-contain"
          controls
          playsInline
          preload="metadata"
          src={previewUrl}
          aria-label={label}
        >
          Seu navegador não suporta a pré-visualização de vídeo.
        </video>
      </div>
    </div>
  );
}
