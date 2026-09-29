"use client";

import { useEffect, useState } from "react";

export function useVideoDuration(previewUrl: string | null): number | null {
  const [duration, setDuration] = useState<number | null>(null);

  useEffect(() => {
    if (!previewUrl) {
      setDuration(null);
      return;
    }

    let cancelled = false;
    const video = document.createElement("video");
    video.preload = "metadata";

    const onLoaded = () => {
      if (!cancelled) {
        setDuration(
          Number.isFinite(video.duration) ? video.duration : null,
        );
      }
    };

    const onError = () => {
      if (!cancelled) setDuration(null);
    };

    video.addEventListener("loadedmetadata", onLoaded);
    video.addEventListener("error", onError);
    video.src = previewUrl;

    return () => {
      cancelled = true;
      video.removeEventListener("loadedmetadata", onLoaded);
      video.removeEventListener("error", onError);
      video.src = "";
    };
  }, [previewUrl]);

  return duration;
}
