# CompressKly

> Monorepo para compressão de vídeos MP4: interface web em Next.js e API em ASP.NET Core com FFmpeg.

<!--
  CompressKly
  Video compression · Next.js · ASP.NET Core · FFmpeg
-->

<p align="left">
  <img src="https://img.shields.io/badge/STATUS-EM_DESENVOLVIMENTO-28545A?style=flat-square" alt="Status do projeto" />
  <img src="https://img.shields.io/badge/Next.js-16-000000?style=flat-square&logo=nextdotjs&logoColor=white" alt="Next.js" />
  <img src="https://img.shields.io/badge/React-19-61DAFB?style=flat-square&logo=react&logoColor=black" alt="React" />
  <img src="https://img.shields.io/badge/TypeScript-5-3178C6?style=flat-square&logo=typescript&logoColor=white" alt="TypeScript" />
  <img src="https://img.shields.io/badge/.NET-10-512BD4?style=flat-square&logo=dotnet&logoColor=white" alt=".NET" />
</p>

---

## Sobre o projeto

O **CompressKly** é uma aplicação full stack para enviar vídeos MP4, escolher o nível de compressão e acompanhar o progresso até o download do arquivo otimizado.

O repositório reúne o **front** (experiência de upload, preview e download) e a **API** (persistência, orquestração e integração com FFmpeg). O objetivo é um fluxo simples e transparente para reduzir o tamanho de vídeos sem depender de serviços de terceiros para o processamento básico.

### Funcionalidades

- **Upload** de MP4 com validação de tamanho e formato
- **Compressão** assíncrona com perfis `high`, `balanced` e `max`
- **Status e progresso** via polling na API
- **Preview** e **download** do vídeo comprimido
- **Documentação OpenAPI** (Swagger) na API
- Monorepo com front e backend versionados juntos

## Preview

<!-- Adicione um print ou GIF em ./docs/preview.png quando disponível.
![Preview do projeto](./docs/preview.png)
-->

Consulte os READMEs de cada parte do monorepo para rodar localmente: [front/README.md](front/README.md) e [api/README.md](api/README.md).

## Tecnologias

As principais tecnologias utilizadas neste projeto são:

- **Next.js 16** — App Router e UI do compressor
- **React 19** — componentes da interface
- **TypeScript** — tipagem no front e consistência com a API
- **Tailwind CSS 4** e **shadcn/ui** — estilização e componentes
- **ASP.NET Core** (.NET 10) — API REST
- **Entity Framework Core** (SQLite) — metadados dos vídeos
- **FluentValidation** — validação de comandos na aplicação
- **FFmpeg / FFprobe** — compressão (dependências externas no ambiente)

## Estrutura do projeto

```text
compresskly/
├── front/          # Next.js — UI de upload, progresso e download
├── api/            # ASP.NET Core — Clean Architecture + FFmpeg
│   ├── src/
│   ├── tests/
│   └── storage/    # Arquivos original/ e compressed/ (runtime)
├── LICENSE
└── README_BASE.md  # Modelo de documentação dos READMEs
```

Detalhes de cada pasta estão em [front/README.md](front/README.md) e [api/README.md](api/README.md).

## Pré-requisitos

Antes de começar, você precisa ter instalado:

- **Node.js** — compatível com Next.js 16
- **npm** — gerenciador de pacotes do front
- **.NET SDK 10+**
- **FFmpeg** e **FFprobe** no PATH (ou caminhos em `appsettings.json` da API)
- **Git**

## Instalação

**1. Clone o repositório**

```bash
git clone https://github.com/nathalyadev/compresskly.git
```

**2. Acesse a pasta**

```bash
cd compresskly
```

**3. API — restore e execução**

```bash
cd api
dotnet restore
dotnet run --project src/CompressKly.Api
```

Swagger: `http://localhost:5080/swagger`

**4. Front — dependências e execução** (em outro terminal)

```bash
cd front
cp .env.example .env.local
npm install
npm run dev
```

A interface estará em `http://localhost:3000`. Defina `NEXT_PUBLIC_API_URL` se a API não estiver em `http://localhost:5080`.

## Scripts e comandos úteis

| Onde | Comando | Descrição |
|---|---|---|
| `front/` | `npm run dev` | Ambiente de desenvolvimento do front. |
| `front/` | `npm run build` | Build de produção do front. |
| `front/` | `npm run lint` | ESLint no front. |
| `api/` | `dotnet run --project src/CompressKly.Api` | Sobe a API. |
| `api/` | `dotnet test` | Testes unitários e de integração. |

## Fluxo da aplicação

1. O front envia o vídeo para `POST /api/videos/upload`
2. O front inicia `POST /api/videos/{id}/compress` com `{ "quality": "balanced" }` (ou `high` / `max`)
3. O front consulta `GET /api/videos/{id}/status` até a compressão concluir
4. Preview e download via `GET /api/videos/{id}/download`

## Decisões técnicas

- **Monorepo:** front e API no mesmo repositório, com README dedicado em cada pasta
- **Arquitetura da API:** camadas Api, Application, Core e Infrastructure; handlers por caso de uso
- **Armazenamento:** arquivos em disco (`storage/`) e metadados em SQLite
- **FFmpeg:** invocado como processo externo; não redistribuído com o projeto ([LICENSE](LICENSE))
- **CORS:** origens configuráveis para o front em desenvolvimento

## Aprendizados e próximos passos

- Separar UI e API facilita evoluir compressão e storage sem acoplar o front
- Possíveis evoluções: deploy documentado, preview visual no README, filas para compressão em escala

## Demonstração

- **Repositório:** [github.com/nathalyadev/compresskly](https://github.com/nathalyadev/compresskly)

## Licença e FFmpeg

O uso deste código é permitido desde que a autora original, **Nathálya**, seja creditada. Os termos completos estão em [LICENSE](LICENSE).

**FFmpeg** não faz parte deste repositório e **não é redistribuído** sob a licença do CompressKly. Quem empacota ou redistribui binários do FFmpeg deve cumprir as licenças dos codecs (por exemplo `libx264`).

## Autoria

Desenvolvido por **Nathálya Souza**.

- **Portfólio:** [nathalyadev.vercel.app](https://nathalyadev.vercel.app/)
- **GitHub:** [@nathalyadev](https://github.com/nathalyadev)
- **LinkedIn:** [nathalya-souza-dev](https://www.linkedin.com/in/nathalya-souza-dev/)

---

<p align="center">
  <sub>CompressKly · compressão de vídeo com Next.js e .NET</sub>
</p>
