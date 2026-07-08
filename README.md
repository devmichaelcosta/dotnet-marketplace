# DotNet Marketplace

Modernizacao de um marketplace legado em PHP para o ecossistema .NET, usando ASP.NET Core, Minimal APIs, Entity Framework Core, ASP.NET Identity/JWT e Blazor.

## Stack

- .NET 10
- ASP.NET Core Minimal APIs
- Entity Framework Core com SQL Server/LocalDB
- ASP.NET Identity + JWT
- Blazor Server
- xUnit

## Estrutura

```text
src/
  Marketplace.Api/   API REST, dominio, Identity, EF Core e migrations
  Marketplace.Web/   Frontend Blazor
tests/
  Marketplace.Tests/ Testes automatizados
legado/              Sistema PHP original usado como referencia funcional
docs/                Inventario funcional e plano de migracao
```

## Requisitos

- .NET SDK 10
- SQL Server LocalDB ou SQL Server
- EF Core CLI, caso precise criar/aplicar migrations manualmente:

```bash
dotnet tool install --global dotnet-ef
```

## Configuracao

A connection string padrao da API esta em `src/Marketplace.Api/appsettings.json`:

```json
"Marketplace": "Server=(localdb)\\SGPLocalDB;Database=DotNetMarketplace;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

Para usar outro SQL Server, altere essa connection string ou sobrescreva por User Secrets/variaveis de ambiente.

Em ambiente Development, `src/Marketplace.Api/appsettings.Development.json` define:

```json
"SeedDatabase": true
```

Isso aplica migrations e cria dados iniciais quando a API inicia.

O frontend consome a API pela chave `ApiBaseUrl`. Em desenvolvimento, use:

```json
"ApiBaseUrl": "http://localhost:5120/"
```

## Executando

Restaurar e compilar:

```bash
dotnet restore DotNetMarketplace.slnx
dotnet build DotNetMarketplace.slnx
```

Rodar a API:

```bash
dotnet run --project src/Marketplace.Api/Marketplace.Api.csproj --urls http://localhost:5120
```

Rodar o Blazor:

```bash
dotnet run --project src/Marketplace.Web/Marketplace.Web.csproj --urls http://localhost:5228
```

Acesse:

- API health: `http://localhost:5120/api/health`
- Web: `http://localhost:5228`

## Banco de dados

Aplicar migrations manualmente, se necessario:

```bash
dotnet ef database update --project src/Marketplace.Api/Marketplace.Api.csproj
```

## Usuarios seed

Quando `SeedDatabase=true`, sao criados usuarios de desenvolvimento:

| Login | Senha | Perfil |
| --- | --- | --- |
| michael | ChangeMe123! | admin |
| multisom | ChangeMe123! | vendedor |
| tatiana | ChangeMe123! | comum |

## Testes

```bash
dotnet test DotNetMarketplace.slnx
```

## Funcionalidades migradas

- Autenticacao e registro de usuarios/vendedores
- Catalogo publico, busca e detalhe de produto
- Carrinho, checkout e pedidos
- Produtos curtidos
- Avaliacoes com aprovacao administrativa
- Admin de usuarios, vendedores, categorias, subcategorias, atributos e produtos
- Perfil do usuario e endereco

## Observacoes

- O sistema ainda preserva a pasta `legado/` como referencia de comportamento.
- Upload real de imagens/assets grava em `wwwroot/uploads`, com validacao de formato e limite de 5 MB por imagem.
- O status consolidado da migracao esta em `docs/status-implementacao.md`.
- Pode haver warning NU1903 em dependencia transitiva `System.Security.Cryptography.Xml`; acompanhar atualizacoes dos pacotes dependentes.
