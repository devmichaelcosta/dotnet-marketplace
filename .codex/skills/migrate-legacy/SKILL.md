---
name: migrate-legacy
description: Analyze and migrate legacy systems to modern .NET while preserving functional behavior, business rules, navigation flows, integrations, data relationships, and user experience. Use when a repository contains legado/, legacy/, php/, or old-system/ folders, or when the user asks to modernize a legacy application into ASP.NET Core, Entity Framework Core, Minimal APIs, and Blazor.
---

# Migrate Legacy

## Objetivo

Analisar sistemas legados e realizar sua modernização para .NET preservando comportamento funcional e regras de negócio enquanto moderniza arquitetura, organização e tecnologias utilizadas.

---

## Processo obrigatório

Ao receber uma solicitação de migração de um sistema legado:

### 1. Analisar o sistema original

Identificar:

* funcionalidades existentes;
* regras de negócio;
* entidades do domínio;
* relacionamentos;
* fluxos de navegação;
* permissões;
* autenticação;
* integrações externas;
* relatórios;
* exportações;
* importações;
* jobs e processamento assíncrono;
* arquivos e documentos gerados;
* APIs consumidas.

Não iniciar implementação antes de entender suficientemente o comportamento atual.

---

### 2. Criar inventário funcional

Gerar uma lista contendo:

* módulos existentes;
* telas;
* operações CRUD;
* regras importantes;
* integrações;
* dependências externas.

Exemplo:

* Usuários
* Produtos
* Pedidos
* Relatórios
* Dashboard
* Controle de permissões

---

### 3. Planejar a migração

Definir:

* ordem de implementação;
* dependências entre módulos;
* estratégia de autenticação;
* estratégia de autorização;
* estratégia de persistência;
* integrações necessárias.

Priorizar:

1. autenticação;
2. entidades principais;
3. funcionalidades centrais;
4. integrações;
5. funcionalidades secundárias.

---

### 4. Não reproduzir limitações técnicas

Preservar:

* comportamento;
* regras de negócio;
* experiência do usuário.

Não preservar automaticamente:

* arquitetura inadequada;
* acoplamento excessivo;
* duplicação de código;
* SQL espalhado;
* lógica em views;
* código procedural.

---

## Plataforma alvo padrão

Quando não especificado pelo usuário utilizar:

### Backend

* .NET 10
* ASP.NET Core
* Minimal APIs
* Entity Framework Core
* SQL Server
* ASP.NET Identity
* JWT Authentication
* FluentValidation

### Frontend

* Blazor
* Componentes reutilizáveis
* Consumo via APIs REST

---

## Arquitetura obrigatória

Utilizar:

* Vertical Slice Architecture
* Organização por funcionalidades
* CQRS quando apropriado
* Dependency Injection
* Logging estruturado

Estrutura esperada:

```text
Features/
    Products/
        Create/
        Update/
        Delete/
        GetById/
        Search/

    Orders/
        Create/
        Cancel/
        List/
        Details/
```

Evitar:

* pasta Services genérica;
* Helpers;
* Utils;
* Controllers gigantes;
* Managers centralizadores.

---

## Implementação de cada funcionalidade

Cada funcionalidade deve conter sempre que necessário:

* Endpoint
* Request
* Response
* Validator
* Handler
* DTOs
* Mapping
* Testes

Cada slice deve ser independente.

---

## Banco de dados

Durante a migração:

* preservar integridade funcional;
* utilizar migrations;
* utilizar Fluent API;
* normalizar estruturas problemáticas quando adequado.

Evitar:

* consultas SQL espalhadas;
* lógica no banco desnecessária;
* procedures novas sem justificativa.

---

## Interface

Objetivos:

* manter aparência semelhante ao sistema legado;
* melhorar responsividade;
* melhorar acessibilidade;
* reutilizar componentes.

Evitar redesenhos completos sem necessidade funcional.

---

## Autonomia

Assuma autonomia para:

* criar entidades;
* criar migrations;
* criar endpoints;
* reorganizar código;
* mover arquivos;
* renomear componentes;
* criar testes;
* corrigir problemas arquiteturais.

Não solicitar confirmação para pequenas decisões técnicas.

Solicitar confirmação apenas quando:

* regras forem conflitantes;
* comportamento original for impossível de determinar;
* múltiplas interpretações forem igualmente válidas.

---

## Fluxo obrigatório de execução

Para cada módulo:

1. analisar implementação legado;
2. identificar regras;
3. identificar dependências;
4. planejar a implementação;
5. implementar;
6. executar build;
7. executar testes;
8. corrigir problemas encontrados;
9. marcar módulo como concluído.

Nunca considerar um módulo finalizado com:

* erros de compilação;
* testes quebrados;
* dependências pendentes.

---

## Critério de conclusão

Uma migração somente será considerada concluída quando:

* todas funcionalidades existirem no novo sistema;
* todas regras de negócio estiverem implementadas;
* o sistema compilar sem erros;
* os principais fluxos estiverem funcionando;
* a arquitetura estiver aderente aos padrões definidos pelo AGENTS.md.

Priorizar sempre qualidade arquitetural, manutenibilidade e escalabilidade de longo prazo.
