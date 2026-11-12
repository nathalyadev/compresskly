# Migrations (CompressKly)

> Notas sobre schema do banco e uso de migrations no projeto Infrastructure.

<!--
  CompressKly · EF Core migrations
-->

---

## Sobre este diretório

Em **desenvolvimento**, o schema SQLite é criado via `EnsureCreated` no startup da API — não há arquivos de migration versionados aqui por padrão.

Em **produção**, o recomendado é versionar migrations geradas pelo EF Core e aplicá-las no deploy.

## Comando para gerar a migration inicial

Execute a partir da pasta `api/`:

```bash
dotnet ef migrations add InitialCreate -p src/CompressKly.Infrastructure -s src/CompressKly.Api -o Data/Migrations
```

## Pré-requisitos

- **.NET SDK 10+**
- Ferramenta global `dotnet-ef` instalada:

```bash
dotnet tool install --global dotnet-ef
```

## Decisões técnicas

- **Dev:** `EnsureCreated` para setup rápido sem migration pendente
- **Prod:** migrations explícitas para evolução controlada do schema

---

<p align="center">
  <sub>CompressKly Infrastructure · Data/Migrations</sub>
</p>
