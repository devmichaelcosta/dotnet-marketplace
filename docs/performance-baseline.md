# Baseline de desempenho

## Ferramenta e seguranca

O benchmark isolado fica em `tools/Marketplace.Benchmarks` e nao faz parte de `DotNetMarketplace.slnx` nem da suite comum de testes. Para restaurar, compilar e executar:

```powershell
dotnet restore tools/Marketplace.Benchmarks/Marketplace.Benchmarks.csproj
dotnet run --project tools/Marketplace.Benchmarks/Marketplace.Benchmarks.csproj
```

A ferramenta so aceita SQL Server LocalDB e um catalogo cujo nome comece com `DotNetMarketplace_Benchmark_`. Antes de aplicar migrations, remove apenas esse banco descartavel e recria o esquema. O nome padrao e unico por execucao. Para selecionar outra instancia local, defina `MARKETPLACE_BENCHMARK_CONNECTION` com uma conexao LocalDB para um catalogo novo com esse prefixo. Nunca configure conexao de homologacao ou producao.

O dataset inicial e sintetico e reproduzivel em volume e atributos: 2.000 usuarios, 10.000 produtos, 10.000 pedidos e 50.000 itens. IDs GUID sao gerados a cada execucao; SKUs, titulos, precos, datas relativas e distribuicoes sao deterministas. O benchmark importa tres planilhas com 500 linhas cada, usando um cliente HTTP local simulado que retorna uma imagem do proprio repositorio, sem trafego externo. Ao final, os registros desse fluxo e do checkout levam as contagens a 11.500 produtos, 2.000 usuarios, 10.030 pedidos e 50.030 itens de pedido.

## Perfis e percentis

Cada perfil executa tres vezes. Os percentis usam o metodo nearest-rank; com apenas tres amostras, p95 e p99 correspondem ao maior valor observado e nao estimam a cauda de uma carga de producao. Throughput representa operacoes por segundo calculadas sobre o tempo total das tres execucoes. As buscas e a importacao executam os handlers diretamente no processo com EF Core. Portanto, incluem a regra do caso de uso e o acesso ao SQL Server, mas excluem rede HTTP, TLS, proxy e renderizacao Blazor.

A importacao processa 500 linhas validas por execucao, com imagem local simulada. O checkout envia 30 requisicoes concorrentes em cada execucao; espera 10 pedidos aceitos, 20 rejeicoes por falta de estoque e estoque final zero.

## Resultado medido

Executado em **2026-10-19 00:56 UTC** (**2026-10-18 21:56 BRT**), no [workflow CI #25](https://github.com/devmichaelcosta/dotnet-marketplace/actions/runs/39182776872), commit `157a2f185590619b9235d18c3081c5906dd3a9c1`.

| Perfil | p50 (ms) | p95 (ms) | p99 (ms) | Throughput (ops/s) | Erros | Timeouts |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Busca publica do catalogo | 69,034 | 255,270 | 255,270 | 7,68 | 0 | 0 |
| Detalhe publico do produto | 2,121 | 119,565 | 119,565 | 24,38 | 0 | 0 |
| Listagem administrativa de produtos | 88,929 | 99,835 | 99,835 | 11,11 | 0 | 0 |
| Listagem administrativa de pedidos | 6,535 | 52,826 | 52,826 | 45,79 | 0 | 0 |
| Importacao de 500 linhas com imagem local simulada | 1.692,557 | 2.059,141 | 2.059,141 | 0,577 | 0 | 0 |
| Checkout concorrente, 30 tentativas por execucao | 55,776 | 232,427 | 232,427 | 129,07 a 614,28 tentativas/s por execucao | 0 | 0 |

Amostras em milissegundos, na ordem em que foram executadas:

- Busca publica: 255,270; 69,034; 66,551.
- Detalhe publico: 119,565; 2,121; 1,350.
- Produtos administrativos: 99,835; 81,173; 88,929.
- Pedidos administrativos: 52,826; 6,535; 6,161.
- Importacao: 2.059,141; 1.447,550; 1.692,557.
- Checkout (lote de 30 requisicoes concorrentes): 232,427; 55,776; 48,838. Os throughputs correspondentes foram 129,07; 537,87; 614,28 tentativas/s.

Em cada uma das tres execucoes de checkout, foram criados 10 pedidos, 20 requisicoes receberam rejeicao de estoque insuficiente e o estoque terminou em zero. O harness encontrou inicialmente atualizacoes perdidas sob concorrencia. A correcao aplica decremento condicional no banco (`Stock >= quantidade`) dentro de transacao explicita, atualiza os produtos em ordem de ID e mantem isolamento `ReadCommitted`; a verificacao passou nas tres repeticoes.

## Ambiente e limites

- Runner: Windows Server 2025 Datacenter, build 26100 (runtime descreve Windows 10.0.26100).
- Runtime .NET: 10.0.13.
- Processadores logicos: 4.
- Memoria disponivel durante a execucao: aproximadamente 16 GiB.
- SQL Server: 2025 Express, versao 17.0.4025.3, CU3.

CPU e memoria sao os recursos disponiveis do runner no momento do benchmark; o relatorio nao atribui consumo exclusivo a cada consulta. Os resultados descrevem este runner de CI com LocalDB e dataset sintetico, nao uma garantia de desempenho ou um SLO de producao. A carga cobre busca publica, detalhe, listagens administrativas, importacao local e consistencia do checkout; nao inclui latencia de rede externa, navegador ou renderizacao da interface.
