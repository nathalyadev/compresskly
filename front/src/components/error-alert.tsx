"use client";

interface ErrorAlertProps {
  message: string;
  onDismiss?: () => void;
}

export function ErrorAlert({ message, onDismiss }: ErrorAlertProps) {
  return (
    <div
      role="alert"
      className="flex w-full items-start justify-between gap-3 rounded-xl border border-[#fce8e6] bg-[#fce8e6] px-4 py-3 text-sm text-[#c5221f]"
    >
      <p>{message}</p>
      {onDismiss ? (
        <button
          type="button"
          onClick={onDismiss}
          className="shrink-0 rounded-md px-2 py-0.5 font-medium text-[#c5221f] hover:bg-[#fad2cf] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#d93025]"
          aria-label="Fechar mensagem de erro"
        >
          Fechar
        </button>
      ) : null}
    </div>
  );
}
