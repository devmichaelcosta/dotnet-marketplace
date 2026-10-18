# Baseline de desempenho

## Ferramenta e seguranca

O benchmark isolado fica em tools/Marketplace.Benchmarks e nao faz parte de DotNetMarketplace.slnx nem da suite de testes comum. Para restaurar, compilar e executar:

    dotnet restore tools/Marketplace.Benchmarks/Marketplace.Benchmarks.csproj
    dotnet run --project tools/Marketplace.Benchmarks/Marketplace.Benchmarks.csproj

A ferramenta so aceita SQL Server LocalDB e um catalogo cujo nome comece com DotNetMarketplace_Benchmark_. Antes de aplicar migrations, ela remove apenas esse banco descartavel e recria o esquema. O nome padrao e unico por execucao. Para selecionar uma instancia local diferente, defina MARKETPLACE_BENCHMARK_CONNECTION para uma conexao LocalDB com um catalogo novo sob esse mesmo prefixo. Nunca configure conexao de homologacao ou producao.

O dataset e sintetico e reproduzivel em volume e atributos: 2.000 usuarios, 10.000 produtos, 10.000 pedidos e 50.000 itens. IDs GUID sao gerados a cada execucao; SKUs, titulos, precos, datas relativas e distribuicoes sao deterministas. O importador processa tres planilhas com 500 linhas cada, usando um HttpClient local simulado que retorna uma imagem do proprio repositorio, sem trafego externo.

## Perfis medidos

Cada consulta/operacao roda tres vezes e registra amostras, p50, p95, p99, throughput, erros e timeouts. Um erro encerra o benchmark e nao e contado como sucesso. Os handlers de busca publica, detalhe, produtos administrativos, pedidos administrativos e importacao sao executados diretamente no processo com EF Core; o relatorio registra essa limitacao de escopo. O checkout dispara 30 requisicoes concorrentes em cada uma de tres execucoes, espera 10 pedidos aceitos e 20 rejeicoes por falta de estoque, e falha se o estoque ficar inconsistente ou surgir status inesperado.

O relatorio inclui sistema operacional, runtime .NET, processadores logicos, memoria disponivel, versao SQL Server e contagens reais do dataset. O JSON e salvo em .tmp/performance-baseline/results.json.

## Resultado desta tentativa

Data: 2026-10-18. A ferramenta compilou sem erros, mas nao chegou a criar o banco ou medir os perfis. No primeiro uso, a instancia MSSQLLocalDB automatica nao existia; ao selecionar a instancia configurada SGPLocalDB, o LocalDB Runtime retornou erro inesperado ao ler a configuracao da instancia no registro, inclusive ao consultar/iniciar a instancia com sqllocaldb. Nenhuma medicao foi produzida e nenhum numero e inferido.

Rerun necessario quando o LocalDB estiver acessivel. O issue #6 permanece aberto ate registrar as tres execucoes de cada perfil, validar a consistencia do checkout e publicar os resultados reais neste documento.
