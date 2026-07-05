# AGENTS.md

## Objetivo do projeto

Este projeto consiste na modernização e evolução de sistemas utilizando tecnologias do ecossistema .NET.

Os agentes possuem autonomia para analisar requisitos, criar funcionalidades, refatorar código e tomar decisões arquiteturais sem necessidade de confirmação constante.

Priorize sempre qualidade arquitetural, manutenibilidade e escalabilidade de longo prazo.

---

## Princípios gerais

Ao implementar qualquer funcionalidade siga obrigatoriamente:

* SOLID
* Clean Code
* DRY
* KISS
* Separation of Concerns
* Convention over Configuration
* Fail Fast
* YAGNI

Evite:

* Classes gigantes
* God Objects
* Controllers com regras de negócio
* Services monolíticos
* Código duplicado
* Dependências desnecessárias
* Métodos excessivamente longos

---

## Arquitetura

Utilizar obrigatoriamente:

* Vertical Slice Architecture
* Organização por funcionalidades
* Baixo acoplamento
* Alta coesão
* Dependency Injection nativa do ASP.NET Core

Estrutura preferencial:

Features/
├── Users/
│   ├── Create/
│   ├── Update/
│   ├── Delete/
│   └── GetById/
│
├── Products/
│   ├── Create/
│   ├── Update/
│   ├── Delete/
│   └── Search/

Cada operação representa um slice independente.

Não criar pastas globais como:

* Controllers
* Services
* Managers
* Helpers
* Utils

Exceto quando forem realmente compartilhadas entre múltiplas features.

---

## Backend

Tecnologias preferenciais:

* .NET 10
* ASP.NET Core
* Minimal APIs
* Entity Framework Core
* ASP.NET Identity
* JWT Authentication
* FluentValidation
* MediatR (ou equivalente)
* AutoMapper apenas quando agregar valor

Toda regra de negócio deve permanecer fora dos endpoints.

Endpoints devem apenas:

* receber requisições;
* delegar processamento;
* retornar respostas.

---

## Frontend

Tecnologia obrigatória:

* Blazor

Diretrizes:

* Componentes pequenos e reutilizáveis.
* Separar lógica da interface sempre que possível.
* Evitar código complexo dentro dos componentes Razor.
* Utilizar serviços para comunicação com APIs.
* Priorizar experiência semelhante ao sistema legado durante migrações.

---

## Banco de dados

Preferências:

* Entity Framework Core.
* Migrations obrigatórias.
* Configurações via Fluent API.
* Evitar Data Annotations excessivas.

Ao migrar sistemas legados:

* preservar regras existentes;
* corrigir inconsistências estruturais quando houver ganho claro;
* documentar alterações relevantes.

---

## APIs

Utilizar:

* REST
* Versionamento quando necessário
* DTOs para entrada e saída
* Validation Pipeline
* ProblemDetails para erros

Nunca expor entidades diretamente pela API.

---

## Tratamento de erros

Utilizar:

* Middleware global de exceções
* Logging estruturado
* Correlação de requests
* Respostas padronizadas

Evitar:

* try/catch desnecessários
* tratamento repetido em handlers

---

## Assincronismo

Sempre preferir:

* async/await
* CancellationToken
* operações não bloqueantes

Evitar:

* .Result
* .Wait()
* Task.Run em código de servidor sem necessidade

---

## Testes

Sempre que possível criar:

* testes unitários para regras críticas;
* testes de integração para endpoints importantes.

Prioridade:

1. regras de negócio;
2. integrações;
3. fluxos críticos.

---

## Migração de sistemas legados

Quando existir uma pasta chamada:

* legado/
* legacy/
* php/
* old-system/

o agente deve:

1. analisar todo o sistema original;
2. identificar funcionalidades existentes;
3. identificar regras de negócio;
4. identificar integrações;
5. identificar entidades e relacionamentos;
6. criar plano de migração;
7. implementar a solução moderna mantendo compatibilidade funcional.

Durante migrações:

* preservar comportamento original;
* não copiar limitações técnicas do sistema antigo;
* modernizar arquitetura;
* melhorar organização do código;
* manter compatibilidade funcional.

---

## Autonomia do agente

O agente possui autonomia para:

* criar entidades;
* criar migrations;
* criar novas features;
* reorganizar código;
* mover arquivos;
* renomear componentes;
* criar testes;
* criar documentação.
* executar comandos auxiliares dentro deste projeto quando necessários para implementar, compilar, testar ou validar a tarefa.

Evite interromper a execução para solicitar pequenas decisões técnicas.

Solicite confirmação apenas quando:

* existirem regras conflitantes;
* houver múltiplas interpretações possíveis;
* informações essenciais estiverem ausentes.

---

## Processo de desenvolvimento

Para cada tarefa:

1. Entender o problema.
2. Localizar código relacionado.
3. Planejar alterações.
4. Implementar.
5. Executar build.
6. Executar testes.
7. Corrigir problemas encontrados.
8. Finalizar a tarefa somente quando o sistema estiver consistente.

Não encerrar tarefas com erros de compilação.

### Limite para comandos auxiliares e servidores locais

Ao tentar subir servidores locais, processos em background, navegadores, smoke tests manuais ou qualquer comando auxiliar que não seja a implementação principal:

* limitar a tentativa a no máximo 2 minutos;
* se não funcionar nesse período, parar a tentativa e mudar de estratégia;
* priorizar build, testes automatizados, análise de logs e implementação;
* não ficar preso em loops de linha de comando para iniciar servidor;
* informar objetivamente o bloqueio e seguir com uma alternativa produtiva.

---

## Critério de qualidade

Uma implementação só deve ser considerada concluída quando:

* compilar sem erros;
* passar nos testes existentes;
* seguir os padrões definidos neste documento;
* manter consistência arquitetural;
* possuir código legível e sustentável.

Qualidade arquitetural possui prioridade sobre velocidade de implementação.
