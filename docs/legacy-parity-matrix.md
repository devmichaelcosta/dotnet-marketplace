# Matriz de paridade com o legado

Status:

- `OK`: fluxo principal migrado e coberto por API/UI.
- `Parcial`: existe implementacao, mas ainda precisa ajuste de comportamento, seguranca, UI ou teste.
- `Falta`: funcionalidade legada ainda nao tem equivalente suficiente.
- `Diferente por seguranca`: comportamento legado foi alterado intencionalmente para evitar risco.

> Nota: esta matriz foi usada durante a migracao e continua como referencia historica.
> O status consolidado atual do projeto esta em `docs/status-implementacao.md`.

| Modulo legado | Referencias PHP | Equivalente .NET | Status | Pendencias para 100% funcional + visual |
| --- | --- | --- | --- | --- |
| Home/catalogo | `HomeController`, `SearchController`, `ProductsByCategoryController`, `ProductsBySubCategoryController`, `views/home`, `views/category` | `CatalogEndpoints`, `Home.razor`, `ProductCard.razor`, `SearchFilterPanel.razor` | Parcial | Conferir paginacao, filtros por categoria/subcategoria e responsividade contra o legado. |
| Detalhe de produto | `DetailsProductController`, `views/produto/details-product*` | `CatalogEndpoints`, `ProductDetails.razor` | Parcial | Validar galeria, atributos, comentarios aprovados, similares e breadcrumbs visualmente. |
| Login/logout/registro | `users/*Login*`, `users/*Register*`, `seller/*Register*` | `AuthEndpoints`, endpoints BFF `/auth/*`, `Login.razor`, `Register.razor`, `Sell.razor` | Parcial | Login usa cookie HTTP-only e antiforgery; revisar mensagens e testes de reload/logout. |
| Perfil/endereco | `UserEdit*`, `AddressCreateController`, `views/admin/users/address.php` | `UserEndpoints`, `Profile.razor` | Parcial | Confirmar regra legada de substituir enderecos e validar campos obrigatorios. |
| Usuarios admin | `users/User*`, `views/admin/users` | `AdminEndpoints`, `AdminUsers.razor` | Parcial | Reset de senha existe; falta smoke completo de criar/editar/remover e mensagens visuais. |
| Vendedores | `sellers/*`, `views/admin/sellers`, `views/seller` | `AdminEndpoints`, `AdminSellers.razor`, `Sell.razor` | Parcial | Confirmar escopo vendedor, pessoa fisica/juridica e detalhes com produtos. |
| Categorias/subcategorias | `categories/*`, `subcategories/*`, `views/admin/categories`, `views/admin/subcategories` | `AdminEndpoints`, `AdminCatalog.razor` | Parcial | Conferir upload de imagem, JSON/listas dependentes e exclusao com produtos desvinculados. |
| Atributos/ficha tecnica | `attributes/*`, `AddProductAttributeController` | `AdminEndpoints`, `AdminCatalog.razor`, `AdminProducts.razor` | Parcial | Validar edicao de valores dinamicos no produto e exibicao no detalhe. |
| Produtos admin | `products/Product*`, `ProductImageUploadController`, `views/admin/product` | `ProductEndpoints`, `AdminProducts.razor` | Parcial | Conferir upload multiplo, SKU unico, estoque, oferta, permissao admin/vendedor e exclusao. |
| Produtos similares | `ProductSimilarController`, `AddSimilarProduct*`, `DeleteSimilarProduct*` | `ProductEndpoints`, `AdminProducts.razor`, `ProductDetails.razor` | Parcial | Validar adicionar/remover similares e exibicao igual ao legado. |
| Curtidas | `LikeProductController`, `DislikeProductController`, `ProductsLikedsController` | `ProductEndpoints`, `LikedProducts.razor`, `ProductDetails.razor` | Parcial | Cobrir idempotencia, descurtir e lista autenticada por teste. |
| Avaliacoes | `ProductRate*`, `ApproveRatingController`, `ListOfRatingPending*` | `ProductEndpoints`, `AdminRatings.razor`, `ProductDetails.razor` | Parcial | Confirmar avaliacao pendente, aprovacao admin e exibicao publica somente aprovada. |
| Carrinho | `Cart*`, `AddToCartController`, `RemoveFromCartController` | `CartEndpoints`, `Cart.razor` | Parcial | Validar merge de carrinho anonimo/autenticado, estoque e mensagens de erro. |
| Checkout | `CartCheckout*`, `OrderService` | `CartEndpoints`, `Cart.razor` | Parcial | Confirmar frete fixo, baixa de estoque, pedido criado, carrinho limpo e nenhum dado sensivel de cartao persistido. |
| Pedidos | `OrderList*`, `OrderDetailsController`, `views/admin/order` | `OrderEndpoints`, `Orders.razor`, `AdminOrders.razor` | Parcial | Testar usuario ve apenas seus pedidos e admin ve todos. |
| Carousel | `carousel/lista.php`, `CarouselRepository` | `AdminEndpoints`, `AdminCarousel.razor`, `Home.razor` | Parcial | Conferir ordenacao, imagem, preview e visual do carrossel contra legado. |
| Uploads | `CategoryImageUploadController`, `ProductImageUploadController`, Dropzone | `AdminEndpoints` upload, `MarketplaceApiClient.UploadImageAsync` | Parcial | Cobrir tamanho, tipo, extensao, path seguro e UX de erro. |
| Seed/db utilitario | `CreateDbController`, `DestroyDbController`, `SeedController` | migrations/seed em startup de desenvolvimento | Diferente por seguranca | Nao expor rotas publicas equivalentes. |
| Pagamento | `Checkout`, `OrderService` | checkout simulado | Diferente por seguranca | Nao persistir numero/CVV/cartao; manter apenas dados nao sensiveis necessarios ao pedido. |

## Ordem de fechamento

1. Autenticacao/autorizacao declarativa nas paginas Blazor e endpoints Web locais com antiforgery.
2. Testes de seguranca e smoke de login/reload/admin.
3. Fluxos criticos: produto, carrinho, checkout, pedidos, perfil e ratings/likes.
4. Auditoria visual das telas publicas e admin contra `legado/src/views`.
5. Testes end-to-end manuais documentados por perfil: anonimo, comum, vendedor e admin.
