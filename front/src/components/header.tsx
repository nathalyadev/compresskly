import { APP_DESCRIPTION, APP_NAME } from "@/lib/constants";

export function Header() {
  return (
    <header className="border-b border-[#e8eaed] bg-white">
      <div className="mx-auto flex w-full max-w-3xl items-center gap-3 px-4 py-4 sm:px-6">
        <div
          className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-[#1a73e8] text-sm font-semibold text-white"
          aria-hidden
        >
          CK
        </div>
        <div className="min-w-0">
          <p className="truncate text-base font-semibold tracking-tight text-[#202124]">
            {APP_NAME}
          </p>
          <p className="truncate text-sm text-[#5f6368]">{APP_DESCRIPTION}</p>
        </div>
      </div>
    </header>
  );
}
