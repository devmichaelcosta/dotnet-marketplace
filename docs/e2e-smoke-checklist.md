# Checklist de smoke E2E

A fixture E2E inicia API e Web em portas efemeras, cria um banco LocalDB exclusivo e usa Microsoft Edge headless. A suite pode ser executada com:

    dotnet test DotNetMarketplace.slnx --filter FullyQualifiedName~Marketplace.Tests.E2E

## Cobertura automatizada registrada

| Perfil | Fluxo | Cobertura |
| --- | --- | --- |
| Anonimo | Abrir home publica e verificar overflow horizontal em 1440x1000 e 390x844 | Anonymous_user_can_open_public_home; Public_home_does_not_overflow_the_viewport |
| Anonimo | Tentar acessar administracao e ser redirecionado ao login | Anonymous_user_is_redirected_to_login_when_accessing_admin_users |
| Cliente | Login e abertura do perfil | Customer_can_login_and_open_profile |
| Vendedor | Login e abertura da gestao de produtos | Seller_can_login_and_open_product_management |
| Admin | Login, listagem e criacao/edicao de produto | Admin_can_login_and_open_product_management; Admin_can_create_product_end_to_end |
| Admin | Editar produto em tema claro e escuro; validar cores, role=status, aria-live e fechamento por teclado | Product_edit_success_notice_remains_readable_in_both_themes |
| Publico | Carregar imagem em oferta, busca e detalhe; validar fallback quando o recurso falha | Product_images_load_in_home_offers_and_search_results |
| Anonimo | Solicitar recuperacao para login existente e inexistente com resposta generica | Request_shows_same_response_for_existing_and_unknown_logins |

## Resultado automatizado desta execucao

Em 2026-10-18, dotnet build DotNetMarketplace.slnx --no-restore -p:UseAppHost=false -m:1 -nr:false concluiu com zero erros. dotnet test DotNetMarketplace.slnx --no-build --no-restore concluiu com 142 aprovados, incluindo os fluxos E2E existentes naquele momento. Depois desse resultado, os testes dos bugs foram ampliados para os dois temas, foco por teclado, fechamento, falha de imagem e carregamento nos tres fluxos. O build dos testes continuou passando. A tentativa de rodar esses E2E ampliados ficou aguardando a API e foi interrompida em dois minutos: o runtime LocalDB retornou erro ao ler a configuracao de SGPLocalDB. Na verificacao mais recente, os 131 testes fora de E2E passaram; o novo E2E precisa ser repetido quando o LocalDB voltar a responder.

## Limites da inspecao visual e de acessibilidade

A auditoria visual manual completa ainda nao foi executada. A cobertura automatizada atual nao percorre todas as paginas administrativas, estados de compra, navegacao por teclado de cada formulario, contraste WCAG AA em cada tema, nem a comparacao de navegacao com o legado. Os dois viewports sao verificados automaticamente apenas na home. Nao declarar a auditoria completa com base neste checklist parcial; a issue #4 permanece aberta ate registrar os fluxos e resultados que faltam.
