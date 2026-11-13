"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { FiHome, FiSettings, FiVideo } from "react-icons/fi";

import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupContent,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarRail,
} from "@/components/ui/sidebar";
import Image from "next/image";
import { BiImage } from "react-icons/bi";

const navItems = [
  { href: "/", label: "Início", icon: FiHome },
  { href: "/videos/compressao", label: "Comprimir", icon: FiVideo },
  { href: "/historico", label: "Histórico", icon: FiHome },
  { href: "/videos", label: "Vídeos", icon: FiVideo },
  { href: "/imagens", label: "Imagens", icon: BiImage },
  { href: "/configuracoes", label: "Configurações", icon: FiSettings },
] as const;

export function AppSidebar() {
  const pathname = usePathname();

  return (
    <Sidebar collapsible="offcanvas" side="left">
      <SidebarHeader>
        <div className="flex items-center gap-3 px-2 py-1">
         <Image src="/images/logo/logo-white.svg" alt="Logo" width={146} height={35} />
        </div>
      </SidebarHeader>

      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupContent>
            <SidebarMenu>
              {navItems.map(({ href, label, icon: Icon }) => (
                <SidebarMenuItem key={href}>
                  <SidebarMenuButton
                    asChild
                    isActive={
                      href === "/"
                        ? pathname === "/"
                        : pathname.startsWith(href)
                    }
                    tooltip={label}
                    size={'lg'}
                  >
                    <Link href={href}>
                      <Icon aria-hidden />
                      <span>{label}</span>
                    </Link>
                  </SidebarMenuButton>
                </SidebarMenuItem>
              ))}
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>
      </SidebarContent>

      <SidebarFooter className="h-15 flex items-center justify-center p-2 text-xs text-sidebar-foreground/70 group-data-[collapsible=icon]:hidden">
        Ctrl+B para alternar o menu
      </SidebarFooter>
      <SidebarRail />
    </Sidebar>
  );
}
