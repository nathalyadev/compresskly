# CompressKly API

> API ASP.NET Core para upload, compressão assíncrona e download de vídeos MP4 com FFmpeg.

<!--
  CompressKly API
  ASP.NET Core · Clean Architecture · FFmpeg · EF Core
-->

<p align="left">
  <img src="https://img.shields.io/badge/STATUS-EM_DESENVOLVIMENTO-28545A?style=flat-square" alt="Status do projeto" />
  <img src="https://img.shields.io/badge/.NET-10-512BD4?style=flat-square&logo=dotnet&logoColor=white" alt=".NET" />
  <img src="https://img.shields.io/badge/ASP.NET_Core-API-512BD4?style=flat-square&logo=dotnet&logoColor=white" alt="ASP.NET Core" />
  <img src="https://img.shields.io/badge/SQLite-003B57?style=flat-square&logo=sqlite&logoColor=white" alt="SQLite" />
</p>

---

## Sobre o projeto

A **CompressKly API** é o backend do monorepo CompressKly. Expõe endpoints REST para receber MP4, enfileirar compressão com perfis de qualidade, informar progresso e servir o arquivo comprimido.

FFmpeg e FFprobe são **dependências externas** do sistema (não incluídas neste repositório). O CompressKly apenas os invoca como processos; binários e codecs seguem as licenças próprias do FFmpeg. O uso do código exige crédito à autora original — veja [LICENSE](../LICENSE).

### Funcionalidades

- **Upload** — `POST /api/videos/upload`
- **Compressão** — `POST /api/videos/{id}/compress` (`high`, `balanced`, `max`)
- **Status** — `GET /api/videos/{id}/status` (progresso e estado)
- **Download** — `GET /api/videos/{id}/download`
- **Swagger** — documentação OpenAPI e respostas de erro padronizadas
- **Testes** — unitários e de integração

## Preview

Documentação interativa após subir a API: [http://localhost:5080/swagger](http://localhost:5080/swagger)

## Tecnologias

As principais tecnologias utilizadas neste projeto são:

- **ASP.NET Core** (.NET 10) — hosts, controllers e middlewares
- **Application layer** — commands/queries, handlers, FluentValidation
- **Entity Framework Core** — SQLite e configurações de entidade
- **FFmpeg / FFprobe** — pipeline de compressão via processo externo
- **Armazenamento local** — pastas `storage/original` e `storage/compressed`

## Estrutura do projeto

```text
api/
├── src/
│   ├── CompressKly.Api/             # Controllers, middlewares, OpenAPI, DI
│   ├── CompressKly.Application/     # Handlers, validadores, interfaces
│   ├── CompressKly.Core/            # Entidades, exceções, contratos compartilhados
│   └── CompressKly.Infrastructure/  # EF Core, FFmpeg, repositórios, storage
├── tests/
│   ├── CompressKly.UnitTests/
│   └── CompressKly.IntegrationTests/
├── storage/
│   ├── original/
│   └── compressed/
└── CompressKly.sln
```

## Pré-requisitos

Antes de começar, você precisa ter instalado:

- **.NET SDK 10+**
- **FFmpeg** e **FFprobe** no PATH ou caminhos configurados em `appsettings.json`
- **Git**

## Instalação

**1. A partir da raiz do monorepo, entre em `api/`**

```bash
cd api
```

**2. Restaure dependências**

```bash
dotnet restore
```

**3. Execute a API**

```bash
dotnet run --project src/CompressKly.Api
```

A API estará disponível na porta configurada em `launchSettings.json` (padrão `http://localhost:5080`).

## Configuração (`appsettings.json`)

| Chave | Descrição |
|---|---|
| `Upload:MaxFileSizeMb` | Limite de upload em megabytes |
| `Storage:RootPath` | Raiz dos arquivos (ex.: `../../storage`) |
| `Ffmpeg:FfmpegPath` / `Ffmpeg:FfprobePath` | Caminhos dos binários |
| `Cors:Origins` | Origens permitidas do front |

## Scripts disponíveis

| Comando | Descrição |
|---|---|
| `dotnet run --project src/CompressKly.Api` | Inicia a API em desenvolvimento. |
| `dotnet test` | Executa todos os testes da solution. |
| `dotnet build` | Compila os projetos da solution. |

## Endpoints

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/videos/upload` | Upload do MP4 |
| `POST` | `/api/videos/{id}/compress` | Inicia compressão (`{ "quality": "balanced" }`) |
| `GET` | `/api/videos/{id}/status` | Status / progresso |
| `GET` | `/api/videos/{id}/download` | Download do arquivo comprimido |

Qualidades aceitas: `high`, `balanced`, `max`.

## Decisões técnicas

- **Clean Architecture:** Core sem dependência de infraestrutura; Application orquestra casos de uso
- **Compressão:** worker assíncrono com FFmpeg; status persistido para polling do client
- **Banco:** SQLite com `EnsureCreated` em desenvolvimento; migrations documentadas em `Data/Migrations/README.md`
- **Erros:** middleware global e contratos OpenAPI para respostas de falha
- **Storage:** filesystem local separando original e comprimido

## Aprendizados e próximos passos

- Isolar FFmpeg atrás de `IVideoCompressor` facilita testes e troca de implementação
- Produção: gerar migrations EF e definir estratégia de volume para `storage/`

## Demonstração

- **Monorepo:** [github.com/nathalyadev/compresskly](https://github.com/nathalyadev/compresskly)
- **Front:** [../front/README.md](../front/README.md)

## Autoria

Desenvolvido por **Nathálya Souza**.

- **Portfólio:** [nathalyadev.vercel.app](https://nathalyadev.vercel.app/)
- **GitHub:** [@nathalyadev](https://github.com/nathalyadev)
- **LinkedIn:** [nathalya-souza-dev](https://www.linkedin.com/in/nathalya-souza-dev/)

---

<p align="center">
  <sub>CompressKly API · ASP.NET Core & FFmpeg</sub>
</p>
