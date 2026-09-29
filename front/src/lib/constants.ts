import type { CompressionQuality } from "@/types/video";

export const APP_NAME = "CompressKly";
export const APP_DESCRIPTION =
  "Comprima vídeos MP4 mantendo boa qualidade visual.";

/** Maximum upload size in MB (client). */
export const MAX_FILE_SIZE_MB = Number(
  process.env.NEXT_PUBLIC_MAX_FILE_SIZE_MB || 500,
);

/** Maximum upload size in bytes (default 500 MB). */
export const MAX_FILE_SIZE_BYTES = MAX_FILE_SIZE_MB * 1024 * 1024;

export const ACCEPTED_EXTENSION = ".mp4";
export const ACCEPTED_MIME_TYPES = [
  "video/mp4",
  "video/x-m4v",
  "application/mp4",
] as const;

export const QUALITY_OPTIONS: {
  value: CompressionQuality;
  label: string;
  description: string;
  estimatedReduction: number;
}[] = [
  {
    value: "high",
    label: "Alta qualidade",
    description: "Melhor imagem, redução moderada",
    estimatedReduction: 0.35,
  },
  {
    value: "balanced",
    label: "Equilibrada",
    description: "Bom equilíbrio entre tamanho e qualidade",
    estimatedReduction: 0.55,
  },
  {
    value: "max",
    label: "Maior compressão",
    description: "Menor arquivo, qualidade aceitável",
    estimatedReduction: 0.7,
  },
];

export const STATUS_MESSAGES: Record<string, string> = {
  preparing: "Preparando vídeo...",
  processing: "Processando...",
  compressing: "Comprimindo...",
  finalizing: "Finalizando...",
  completed: "Compressão concluída",
  error: "Erro no processamento",
};

export const ERROR_MESSAGES = {
  notMp4: "O arquivo selecionado não é um vídeo MP4 válido.",
  tooLarge: `O arquivo excede o tamanho máximo permitido (${MAX_FILE_SIZE_MB} MB).`,
  uploadInterrupted: "O envio do arquivo foi interrompido. Tente novamente.",
  processingFailed: "Não foi possível comprimir o vídeo. Tente novamente.",
  ffmpegUnavailable:
    "O processador de vídeo está indisponível no momento. Tente mais tarde.",
  outputFailed: "Falha ao gerar o arquivo final. Tente novamente.",
  downloadFailed: "Não foi possível baixar o vídeo comprimido. Tente novamente.",
  generic: "Ocorreu um erro inesperado. Tente novamente.",
} as const;
