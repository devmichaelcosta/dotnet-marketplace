# Checklist de smoke E2E

Os fluxos abaixo usam a mesma base de teste com banco LocalDB isolado por execução. A suíte pode ser executada com:

```bash
dotnet test DotNetMarketplace.slnx --filter FullyQualifiedName~Marketplace.Tests.E2E
```

## Cobertura automatizada

| Perfil | Fluxo | Cobertura |
| --- | --- | --- |
| Anônimo | Abrir home pública | `Anonymous_user_can_open_public_home` |
| Anônimo | Tentar acessar administração e ser redirecionado ao login | `Anonymous_user_is_redirected_to_login_when_accessing_admin_users` |
| Anônimo | Home sem overflow horizontal em desktop e mobile | `Public_home_does_not_overflow_the_viewport` |
| Comum | Login e abertura do perfil | `Customer_can_login_and_open_profile` |
| Vendedor | Login e abertura da gestão de produtos | `Seller_can_login_and_open_product_management` |
| Admin | Login e abertura da gestão de produtos | `Admin_can_login_and_open_product_management` |
| Admin | Criar produto, pesquisar e reabrir edição | `Admin_can_create_product_end_to_end` |

## Checklist manual visual

Após alterações de CSS ou componentes, revisar em viewport desktop e mobile:

- home, busca e detalhe do produto;
- login, registro, recuperação e perfil;
- carrinho vazio, carrinho com itens e checkout;
- pedidos e produtos curtidos;
- tabelas administrativas, filtros por `Enter`, ordenação e ações por ícone;
- upload de imagens, carrossel e importação de planilha;
- mensagens de sucesso/erro e navegação por teclado.

O checklist manual complementa os testes automatizados porque responsividade, contraste e alinhamento visual não são garantidos apenas pela suíte de comportamento.

## Resultado desta rodada

- A regressão automatizada de viewport passou em `1440x1000` e `390x844`.
- A inspeção manual completa não foi concluída porque a instância local `SGPLocalDB` não pôde ser inicializada pelo runtime do Windows nesta máquina.
