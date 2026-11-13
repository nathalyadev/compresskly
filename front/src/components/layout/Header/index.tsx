"use client";

import { SidebarTrigger } from "@/components/ui/sidebar";
import Image from "next/image";

export function Header() {
  return (
    <header className="flex h-15 shrink-0 items-center gap-2 border-b border-border bg-card px-4 sm:px-6">
      <SidebarTrigger className="-ml-1 md:hidden" />
      <div className="flex min-w-0 flex-1 items-center gap-3">
        <Image src="/images/logo/logo.svg" alt="Logo" width={146} height={35} />
      </div>
    </header>
  );
}
