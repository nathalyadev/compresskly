"use client";

import { Download } from "lucide-react";
import { Button } from "@/components/ui/button";
import { formatBytes, formatPercent } from "@/lib/utils";
import type { CompressionResult } from "@/types/video";

interface CompressionResultCardProps {
  result: CompressionResult;
  previewUrl: string;
  onDownload: () => void;
  onReset: () => void;
  downloading?: boolean;
}

export function CompressionResultCard({
  result,
  previewUrl,
  onDownload,
  onReset,
  downloading = false,
}: CompressionResultCardProps) {
  return (
    <section className="w-full overflow-hidden rounded-2xl border border-[#e8eaed] bg-white">
      <div className="border-b border-[#e8eaed] px-4 py-4 sm:px-6">
        <h2 className="text-lg font-medium text-[#202124]">Vídeo comprimido</h2>
        <p className="mt-1 text-sm text-[#5f6368]">
          Seu arquivo está pronto para download.
        </p>
      </div>

      <div className="grid grid-cols-2 gap-4 px-4 py-5 sm:grid-cols-4 sm:px-6">
        <Stat label="Original" value={formatBytes(result.originalSize)} />
        <Stat label="Comprimido" value={formatBytes(result.compressedSize)} />
        <Stat label="Economia" value={formatBytes(result.savedBytes)} />
        <Stat
          label="Redução"
          value={formatPercent(result.reductionPercent)}
        />
      </div>

      <div className="bg-[#202124]">
        <video
          className="aspect-video w-full bg-black object-contain"
          controls
          playsInline
          preload="metadata"
          src={previewUrl}
          aria-label="Pré-visualização do vídeo comprimido"
        >
          Seu navegador não suporta a pré-visualização de vídeo.
        </video>
      </div>

      <div className="flex flex-col gap-3 px-4 py-5 sm:flex-row sm:px-6">
        <Button
          type="button"
          className="w-full sm:flex-1"
          onClick={onDownload}
          disabled={downloading}
          aria-label="Baixar vídeo comprimido"
        >
          <Download aria-hidden />
          {downloading ? "Baixando..." : "Baixar vídeo"}
        </Button>
        <Button
          type="button"
          variant="outline"
          className="w-full sm:flex-1"
          onClick={onReset}
        >
          Comprimir outro vídeo
        </Button>
      </div>
    </section>
  );
}

function Stat({ label, value }: { label: string; value: string }) {
  return (
    <div>
      <p className="text-xs uppercase tracking-wide text-[#5f6368]">{label}</p>
      <p className="mt-1 text-lg font-semibold tabular-nums text-[#202124]">
        {value}
      </p>
    </div>
  );
}
