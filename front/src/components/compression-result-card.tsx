"use client";

import { Download } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
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
    <Card className="overflow-hidden py-0">
      <CardHeader className="border-b pt-4">
        <div className="flex items-center gap-2">
          <CardTitle className="text-lg">Vídeo comprimido</CardTitle>
          <Badge variant="secondary">Pronto</Badge>
        </div>
        <CardDescription>
          Seu arquivo está pronto para download.
        </CardDescription>
      </CardHeader>

      <CardContent className="grid grid-cols-2 gap-4 py-5 sm:grid-cols-4">
        <Stat label="Original" value={formatBytes(result.originalSize)} />
        <Stat label="Comprimido" value={formatBytes(result.compressedSize)} />
        <Stat label="Economia" value={formatBytes(result.savedBytes)} />
        <Stat
          label="Redução"
          value={formatPercent(result.reductionPercent)}
        />
      </CardContent>

      <Separator />

      <div className="bg-foreground">
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

      <CardFooter className="flex flex-col gap-3 sm:flex-row">
        <Button
          type="button"
          size="lg"
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
          size="lg"
          className="w-full sm:flex-1"
          onClick={onReset}
        >
          Comprimir outro vídeo
        </Button>
      </CardFooter>
    </Card>
  );
}

function Stat({ label, value }: { label: string; value: string }) {
  return (
    <div>
      <p className="text-xs uppercase tracking-wide text-muted-foreground">
        {label}
      </p>
      <p className="mt-1 text-lg font-semibold tabular-nums text-foreground">
        {value}
      </p>
    </div>
  );
}
