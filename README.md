# CompressKly — monorepo

## Estrutura

```text
front/          # Next.js + React + TypeScript + Tailwind
api/            # ASP.NET Core (C#) — Clean Architecture
```

## Front (`front/`)

UI de upload, preview, progresso e download. Consome a API C#.

```bash
cd front
cp .env.example .env.local
npm install
npm run dev
```

Abra [http://localhost:3000](http://localhost:3000).

Variável importante: `NEXT_PUBLIC_API_URL` (padrão `http://localhost:5080`).

## API (`api/`)

Backend **CompressKly** em .NET 10, com camadas Domain / Application / Infrastructure / Api.

### Pré-requisitos

- .NET SDK 10+
- FFmpeg e FFprobe no PATH (ou configure `Ffmpeg:FfmpegPath` / `Ffmpeg:FfprobePath` no `appsettings.json`)

### Executar

```bash
cd api
dotnet run --project src/CompressKly.Api
```

Swagger: [http://localhost:5080/swagger](http://localhost:5080/swagger)

### Endpoints

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/videos/upload` | Upload do MP4 |
| `POST` | `/api/videos/{id}/compress` | Inicia compressão (`{ "quality": "balanced" }`) |
| `GET` | `/api/videos/{id}/status` | Status / progresso |
| `GET` | `/api/videos/{id}/download` | Download do arquivo comprimido |

Qualidades: `high`, `balanced`, `max`.

### Testes

```bash
cd api
dotnet test
```

## Fluxo

1. Front envia o vídeo para `POST /api/videos/upload`
2. Front inicia `POST /api/videos/{id}/compress`
3. Front faz polling em `GET /api/videos/{id}/status`
4. Ao concluir, permite preview/download via `GET /api/videos/{id}/download`

## Stack

- **Front:** Next.js, React, TypeScript, Tailwind
- **API:** ASP.NET Core, EF Core (SQLite), FluentValidation, FFmpeg
