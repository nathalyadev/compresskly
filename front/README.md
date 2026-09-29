# CompressKly Front

Interface web do CompressKly (Next.js + React + TypeScript + Tailwind + shadcn/ui).

## Desenvolvimento

```bash
cp .env.example .env.local
npm install
npm run dev
```

Configure `NEXT_PUBLIC_API_URL` apontando para a API (padrão `http://localhost:5080`).

## shadcn/ui

O projeto usa shadcn/ui (estilo `radix-nova`) com Tailwind v4.

Componentes em `src/components/ui/`:

```bash
npx shadcn@latest add [component]
```

## Scripts

```bash
npm run dev
npm run build
npm run start
npm run lint
```
