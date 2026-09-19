# Status de Implementação

## Visão geral

O marketplace legado em PHP já foi modernizado em uma base .NET com API, Blazor e testes automatizados. O objetivo agora é manter um documento simples de acompanhamento para distinguir o que está concluído, o que foi alterado por segurança e o que ainda merece atenção futura.

## Concluído

- Autenticação, registro e logout.
- Perfil do usuário e gerenciamento de endereços.
- Catálogo público, home, busca e detalhe de produto.
- Admin de usuários, vendedores, categorias, subcategorias e atributos.
- Admin de produtos, imagens, similares, curtidas e avaliações.
- Carrinho, checkout e pedidos.
- Importação de produtos por planilha.
- Frontend Blazor para os fluxos principais.
- Testes automatizados cobrindo os fluxos mais importantes.

## Concluído e validado

- `dotnet test DotNetMarketplace.slnx` executado com sucesso.
- A estrutura de slices está organizada por funcionalidade.
- O sistema compila e os principais fluxos possuem cobertura automatizada.
- Checkout validado no servidor, com estado/endereço/documentos obrigatórios, transação serializável para evitar baixa concorrente de estoque e sem persistência de dados de cartão.
- Índices adicionados para SKU, catálogo, pedidos, avaliações, itens de pedido e produtos similares.
- Suíte E2E validada para perfis anônimo, comum, vendedor e admin.
- Casos de borda adicionados para carrinho sem estoque e importações com cabeçalhos/valores inválidos.
- Revisão de performance aplicada às leituras públicas/admin e ao importador, com consultas sem tracking e downloads de imagens concorrentes limitados a quatro tarefas.
- Regressão automatizada de responsividade validada em `1440x1000` e `390x844`.

## Diferente por segurança

- Rotas de criação/exclusão de banco e seed público do legado não foram reproduzidas.
- Dados sensíveis de cartão não são persistidos como no sistema antigo.
- A autenticação moderna usa o fluxo atual da solução .NET, preservando o comportamento funcional sem copiar a superfície insegura do legado.

## Ainda monitorar

- Ajustes visuais finos entre a interface Blazor e o legado original; a inspeção manual final depende da disponibilidade do LocalDB para subir a aplicação completa.
- Eventuais melhorias incrementais de cobertura para fluxos raros de borda.
- Medição de carga com volumes de produção reais em listas muito grandes e importações extensas.
- Inspeção manual de contraste, alinhamento e navegação por teclado em diferentes resoluções.

O roteiro de validação está em `docs/e2e-smoke-checklist.md`.

## Observação final

A matriz de paridade continua como referência histórica da migração. Este documento representa o estado consolidado atual da implementação.
