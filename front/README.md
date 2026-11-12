# CompressKly Front

> Interface web do CompressKly para upload, escolha de qualidade, acompanhamento da compressão e download.

<!--
  CompressKly Front
  Next.js · React · Tailwind · shadcn/ui
-->

<p align="left">
  <img src="https://img.shields.io/badge/STATUS-EM_DESENVOLVIMENTO-28545A?style=flat-square" alt="Status do projeto" />
  <img src="https://img.shields.io/badge/Next.js-16-000000?style=flat-square&logo=nextdotjs&logoColor=white" alt="Next.js" />
  <img src="https://img.shields.io/badge/React-19-61DAFB?style=flat-square&logo=react&logoColor=black" alt="React" />
  <img src="https://img.shields.io/badge/TypeScript-5-3178C6?style=flat-square&logo=typescript&logoColor=white" alt="TypeScript" />
  <img src="https://img.shields.io/badge/Tailwind_CSS-4-06B6D4?style=flat-square&logo=tailwindcss&logoColor=white" alt="Tailwind CSS" />
</p>

---

## Sobre o projeto

O **CompressKly Front** é a camada de apresentação do monorepo. Conecta-se à API C# para enviar vídeos MP4, selecionar o perfil de compressão e exibir progresso, preview e resultado final.

A experiência prioriza feedback claro durante o upload e a compressão, validação no client e componentes reutilizáveis (shadcn/ui).

### Funcionalidades

- **Zona de upload** com validação de arquivo
- **Seletor de qualidade** (`high`, `balanced`, `max`)
- **Painel de progresso** com polling do status na API
- **Preview** e card de resultado com download
- **Tratamento de erros** com alertas na interface
- Cliente HTTP tipado em `lib/api-client.ts`

## Preview

<!-- Adicione um print ou GIF em ./docs/preview.png quando disponível.
![Preview do front](./docs/preview.png)
-->

Com a API em execução, abra [http://localhost:3000](http://localhost:3000).

## Tecnologias

As principais tecnologias utilizadas neste projeto são:

- **Next.js 16** — App Router, layout e página principal
- **React 19** — composição de componentes
- **TypeScript** — tipos de vídeo e contratos com a API
- **Tailwind CSS 4** — estilização utilitária
- **shadcn/ui** (estilo `radix-nova`) — Button, Card, Progress, Alert, etc.
- **react-icons** (Feather / MIT) — ícones

## Estrutura do projeto

```text
front/
├── src/
│   ├── app/              # layout.tsx, page.tsx, globals.css
│   ├── components/       # compressor, upload, progresso, preview…
│   │   └── ui/           # componentes shadcn
│   ├── hooks/            # ex.: duração do vídeo no client
│   ├── lib/              # api-client, constants, validation, utils
│   └── types/            # tipos compartilhados (video)
├── .env.example
└── package.json
```

## Pré-requisitos

Antes de começar, você precisa ter instalado:

- **Node.js** — compatível com Next.js 16
- **npm**
- **API CompressKly** rodando (veja [../api/README.md](../api/README.md))
- **Git**

## Instalação

**1. A partir da raiz do monorepo, entre em `front/`**

```bash
cd front
```

**2. Configure variáveis de ambiente**

```bash
cp .env.example .env.local
```

**3. Instale as dependências**

```bash
npm install
```

**4. Execute o projeto**

```bash
npm run dev
```

A aplicação estará disponível em `http://localhost:3000`.

Variável importante: `NEXT_PUBLIC_API_URL` (padrão `http://localhost:5080`).

## Scripts disponíveis

| Comando | Descrição |
|---|---|
| `npm run dev` | Inicia o ambiente de desenvolvimento. |
| `npm run build` | Gera a versão de produção. |
| `npm run start` | Inicia a aplicação em produção (após `build`). |
| `npm run lint` | Executa ESLint. |

## shadcn/ui

O projeto usa shadcn/ui com Tailwind v4. Para adicionar componentes:

```bash
npx shadcn@latest add [component]
```

Os componentes gerados ficam em `src/components/ui/`.

## Decisões técnicas

- **App Router:** página única com fluxo guidado (upload → compressão → resultado)
- **API client centralizado:** URLs e fetch em `lib/api-client.ts`
- **Polling de status:** hook e painel de progresso desacoplados do upload
- **Validação:** regras compartilhadas em `lib/validation.ts` antes do envio
- **UI:** primitives shadcn para consistência visual e acessibilidade básica

## Aprendizados e próximos passos

- Tipar respostas da API reduz erros no fluxo de compressão
- Possíveis melhorias: cancelamento de job, estimativa de tempo e tema escuro

## Demonstração

- **Monorepo:** [github.com/nathalyadev/compresskly](https://github.com/nathalyadev/compresskly)
- **API:** [../api/README.md](../api/README.md)

## Autoria

Desenvolvido por **Nathálya Souza**.

- **Portfólio:** [nathalyadev.vercel.app](https://nathalyadev.vercel.app/)
- **GitHub:** [@nathalyadev](https://github.com/nathalyadev)
- **LinkedIn:** [nathalya-souza-dev](https://www.linkedin.com/in/nathalya-souza-dev/)

---

<p align="center">
  <sub>CompressKly Front · Next.js & UI Engineering</sub>
</p>
