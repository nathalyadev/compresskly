import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import { Header } from "@/components/header";
import { APP_DESCRIPTION, APP_NAME } from "@/lib/constants";
import "./globals.css";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: `${APP_NAME} — Compressão de vídeos MP4`,
  description: APP_DESCRIPTION,
};

export default function RootLayout({ children }: LayoutProps<"/">) {
  return (
    <html
      lang="pt-BR"
      className={`${geistSans.variable} ${geistMono.variable} h-full antialiased`}
    >
      <body className="flex min-h-full flex-col bg-[#f8f9fa] text-[#202124]">
        <Header />
        <main className="flex-1">{children}</main>
        <footer className="border-t border-[#e8eaed] bg-white">
          <div className="mx-auto max-w-3xl px-4 py-4 text-sm text-[#5f6368] sm:px-6">
            Os vídeos são processados temporariamente e removidos após o
            download ou expiração.
          </div>
        </footer>
      </body>
    </html>
  );
}
