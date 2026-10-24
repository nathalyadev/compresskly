# CompressKly API

API ASP.NET Core para upload e compressão de vídeos MP4 com FFmpeg.

FFmpeg/FFprobe são dependências **externas** do sistema (não incluídas neste repositório). O CompressKly apenas os invoca como processos; binários e codecs (ex.: `libx264`) seguem as licenças próprias do FFmpeg e não são redistribuídos sob a licença deste projeto. O uso do código do CompressKly exige crédito à autora original, Nathálya ([LICENSE](../LICENSE)).

## Arquitetura

```text
src/
  CompressKly.Api/             # Controllers, middlewares, DI
  CompressKly.Application/     # Commands/Queries, handlers, DTOs, interfaces
  CompressKly.Domain/          # Entities, enums, exceptions
  CompressKly.Infrastructure/  # EF Core, FFmpeg, storage local, repositórios
tests/
  CompressKly.UnitTests/
  CompressKly.IntegrationTests/
storage/
  original/
  compressed/
```

## Executar

```bash
dotnet restore
dotnet run --project src/CompressKly.Api
```

## Configuração (`appsettings.json`)

- `Upload:MaxFileSizeMb` — limite de upload
- `Storage:RootPath` — pasta de arquivos (`../../storage` a partir do projeto Api)
- `Ffmpeg:FfmpegPath` / `Ffmpeg:FfprobePath` — caminhos dos binários
- `Cors:Origins` — origens permitidas (front)

## Testes

```bash
dotnet test
```
