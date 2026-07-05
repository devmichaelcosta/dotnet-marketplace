# Inventario funcional e plano de migracao

## Escopo analisado

Sistema legado em `legado/src`, implementado em PHP sem framework, com arquitetura MVC caseira, roteamento manual, views PHP, JavaScript com jQuery, Dropzone e SweetAlert2, persistencia MySQL via PDO e sessoes PHP.

Arquivos principais analisados:

- `legado/src/index.php`
- `legado/src/infra/configs/router.php`
- `legado/src/script-database-generate.sql`
- `legado/src/controllers/**`
- `legado/src/services/**`
- `legado/src/infra/repositories/**`
- `legado/src/models/**`
- `legado/src/domain/**`
- `legado/src/views/**`
- `legado/src/assets/js/**`

## Visao geral do sistema

O sistema simula um marketplace com area publica de catalogo, area administrativa, cadastro de usuarios, cadastro de vendedores, gerenciamento de produtos, carrinho de compras, checkout, pedidos, curtidas, avaliacoes, categorias, subcategorias e atributos de ficha tecnica.

O ponto de entrada e `index.php`. Ele inicializa autoload, cria `MySqlRepositoryFactory`, inicia sessao, resolve `PATH_INFO`, consulta a tabela de rotas em `infra/configs/router.php`, verifica autorizacao por role e instancia o controller alvo.

## Perfis e autorizacao

Perfis encontrados:

- `admin`
- `vendedor`
- `comum`
- anonimo, quando a rota nao exige roles

Autorizacao atual:

- Baseada em `$_SESSION["role"]`.
- Rotas declaram roles como string separada por virgula.
- Rotas sem role sao publicas.
- Rotas restritas redirecionam para `/login` quando nao autorizadas.

Dados de sessao relevantes:

- `userId`
- `userName`
- `role`
- `sellerId`
- `cart`

## Modulos funcionais

### Home e catalogo publico

Responsabilidades:

- Exibir home do marketplace.
- Exibir carrossel.
- Exibir categorias.
- Listar produtos.
- Pesquisar produtos.
- Listar produtos por categoria e subcategoria.
- Exibir detalhes do produto.
- Exibir imagens, atributos, comentarios/avaliacoes e produtos similares.

Rotas principais:

- `/`
- `/pesquisa`
- `/detalhes-produto`
- `/categorias`
- `/produto/subcategoria`

Views:

- `views/home/home.php`
- `views/home/pesquisa.php`
- `views/category/lista.php`
- `views/carousel/lista.php`
- `views/produto/details-product.php`
- `views/produto/details-product-images.php`
- `views/produto/details-product-attributes.php`
- `views/produto/details-product-comments.php`
- `views/produto/card-list.php`
- `views/produto/card-similar-product.php`
- `views/produto/list-by-categories.php`
- `views/produto/list-by-subcategories.php`

### Autenticacao e usuarios

Responsabilidades:

- Login de usuario.
- Logout.
- Registro publico de usuario comum.
- CRUD administrativo de usuarios.
- Edicao do proprio usuario por `admin`, `vendedor` ou `comum`.
- Enderecos vinculados ao usuario.

Rotas principais:

- `/login`
- `/autenticar`
- `/logout`
- `/registrar`
- `/registrar-usuario-post`
- `/admin/usuario`
- `/admin/usuario/lista-table`
- `/admin/usuario/cadastrar`
- `/admin/usuario/cadastrar-post`
- `/admin/usuario/editar`
- `/admin/usuario/editar-post`
- `/admin/usuario/deletar`
- `/admin/endereco/cadastrar`

Views:

- `views/usuario/login.php`
- `views/usuario/registrar.php`
- `views/admin/users/lista-usuario.php`
- `views/admin/users/lista-usuarios-table.php`
- `views/admin/users/cadastrar-usuario.php`
- `views/admin/users/editar-usuario.php`
- `views/admin/users/address.php`

Regras observadas:

- Login deve localizar usuario por login e verificar senha com hash Argon2i.
- Registro comum cria usuario com role `comum`.
- Login duplicado deve ser rejeitado.
- Edicao de role so pode ser feita por usuario `admin`.
- Ao editar usuario, enderecos antigos sao removidos e recriados.

### Vendedores

Responsabilidades:

- Cadastro publico de vendedor.
- Identificacao/login de vendedor.
- CRUD administrativo de vendedores.
- Edicao do proprio vendedor.
- Detalhes do vendedor com seus produtos.
- Diferenciacao entre vendedor pessoa fisica e juridica.

Rotas principais:

- `/vender`
- `/vendedor-indentificacao`
- `/vendedor-registrar`
- `/vendedor-registrar-post`
- `/admin/vendedor`
- `/admin/vendedor/lista-table`
- `/admin/vendedor/cadastrar`
- `/admin/vendedor/cadastrar-post`
- `/admin/vendedor/editar`
- `/admin/vendedor/editar-post`
- `/admin/vendedor/deletar`
- `/admin/vendedor/detalhes`

Views:

- `views/seller/login.php`
- `views/seller/register.php`
- `views/admin/sellers/lista.php`
- `views/admin/sellers/lista-table.php`
- `views/admin/sellers/cadastrar.php`
- `views/admin/sellers/editar.php`
- `views/admin/sellers/detalhes.php`

Regras observadas:

- Registro de vendedor cria usuario com role `vendedor`.
- Apos criar usuario vendedor, cria registro simplificado em `sellers`.
- Vendedor logado recebe `sellerId` na sessao.
- Edicao de vendedor alterna campos conforme pessoa juridica (`cnpj`, `company`, `branchOfActivity`, `fantasyName`) ou pessoa fisica (`age`, `dateOfBirth`).
- Edicao de vendedor tambem atualiza endereco e dados basicos do usuario.

### Produtos

Responsabilidades:

- CRUD de produtos por admin e vendedor.
- Vendedor lista apenas seus produtos.
- Admin pode escolher vendedor dono do produto.
- Upload de imagens de produto.
- Controle de estoque.
- SKU.
- Produto em oferta.
- Associacao com subcategoria.
- Ficha tecnica por atributos dinamicos.
- Produtos similares.
- Curtidas.
- Avaliacoes pendentes e aprovacao de avaliacoes.

Rotas principais:

- `/admin/produto`
- `/admin/produto/lista-partial`
- `/admin/produto/cadastrar`
- `/admin/produto/cadastrar-post`
- `/admin/produto/editar`
- `/admin/produto/editar-post`
- `/admin/produto/deletar`
- `/admin/produto/upload`
- `/admin/produto/atributo/adicionar`
- `/admin/produto/similares`
- `/admin/produto/similares/lista-partial`
- `/admin/produto/similares/add`
- `/admin/produto/similares/add-post`
- `/admin/produto/similares/deletar`
- `/produto/curtir`
- `/produto/descurtir`
- `/produto/curtidos`
- `/produto/avaliar`
- `/produto/avaliar-post`
- `/admin/avaliacoes-pendentes`
- `/admin/produto/lista-avaliacoes-pendentes-partial`
- `/admin/produto/aprovar-avaliacao`

Views:

- `views/admin/product/lista.php`
- `views/admin/product/lista-table.php`
- `views/admin/product/cadastrar.php`
- `views/admin/product/editar.php`
- `views/admin/product/add-attribute.php`
- `views/admin/product/similar.php`
- `views/admin/product/add-similar.php`
- `views/admin/product/list-similar-products.php`
- `views/admin/product/list-similar-products-of-product.php`
- `views/admin/product/rating-pending.php`
- `views/admin/product/list-rating-pending.php`
- `views/produto/likeds.php`
- `views/produto/product-rating.php`

Regras observadas:

- Produto exige titulo, preco, estoque, oferta, SKU e descricao.
- Criacao persiste produto, imagens e valores de atributos.
- Edicao remove imagens e atributos antigos e recria os atuais.
- Remocao de produto deve limpar dependencias relacionadas.
- Curtida insere apenas se ainda nao existir curtida do mesmo usuario/produto.
- Descurtida remove o registro de curtida.
- Avaliacao exige titulo e descricao.
- Avaliacao nova entra com aprovacao pendente; admin aprova posteriormente.
- Produtos similares sao mantidos em associacao pai/filho.

### Categorias e subcategorias

Responsabilidades:

- CRUD administrativo de categorias.
- Upload de imagem de categoria.
- Listagem JSON de categorias para formularios.
- CRUD administrativo de subcategorias.
- Listagem JSON de subcategorias por categoria.

Rotas principais:

- `/admin/categoria`
- `/admin/categoria/lista-table`
- `/admin/categoria/lista-json`
- `/admin/categoria/cadastrar`
- `/admin/categoria/cadastrar-post`
- `/admin/categoria/editar`
- `/admin/categoria/editar-post`
- `/admin/categoria/deletar`
- `/admin/categoria/upload`
- `/admin/subcategoria`
- `/admin/subcategoria/lista-table`
- `/admin/subcategoria/lista-json`
- `/admin/subcategoria/cadastrar`
- `/admin/subcategoria/cadastrar-post`
- `/admin/subcategoria/editar`
- `/admin/subcategoria/editar-post`
- `/admin/subcategoria/deletar`

Views:

- `views/admin/categories/lista.php`
- `views/admin/categories/lista-table.php`
- `views/admin/categories/cadastrar.php`
- `views/admin/categories/editar.php`
- `views/admin/subcategories/lista.php`
- `views/admin/subcategories/lista-table.php`
- `views/admin/subcategories/cadastrar.php`
- `views/admin/subcategories/editar.php`
- `views/admin/subcategories/buttons-in-list.php`

Regras observadas:

- Categoria exige titulo.
- Ao excluir categoria, produtos de suas subcategorias ficam com `SubCategoryId = null`, subcategorias sao removidas e depois a categoria.
- Subcategoria exige titulo e pertence a uma categoria.
- Ao excluir subcategoria, produtos ficam com `SubCategoryId = null`.

### Atributos e ficha tecnica

Responsabilidades:

- CRUD administrativo de atributos.
- Atributos sao nomes reutilizaveis de ficha tecnica.
- Produtos armazenam pares atributo/valor em `attributevalues`.

Rotas principais:

- `/admin/atributo`
- `/admin/atributo/lista-table`
- `/admin/atributo/cadastrar`
- `/admin/atributo/cadastrar-post`
- `/admin/atributo/editar`
- `/admin/atributo/editar-post`
- `/admin/atributo/deletar`

Views:

- `views/admin/attributes/index.php`
- `views/admin/attributes/list.php`
- `views/admin/attributes/create.php`
- `views/admin/attributes/edit.php`

Regras observadas:

- Atributo exige nome.
- Valor de atributo exige valor.
- Produto pode ter multiplos atributos dinamicos.

### Carrinho e checkout

Responsabilidades:

- Adicionar produto ao carrinho.
- Listar carrinho.
- Remover item.
- Atualizar quantidade.
- Validar estoque.
- Checkout com endereco e dados de pagamento.
- Geracao de pedido.
- Baixa de estoque.

Rotas principais:

- `/carrinho`
- `/listar-itens-carrinho`
- `/adicionar-carrinho`
- `/remover-item-carrinho`
- `/atualizar-quantidade-produto`
- `/finalizar-pedido`
- `/cart-checkout-post`

Views:

- `views/home/carrinho.php`
- `views/home/carrinho-itens.php`
- `views/home/carrinho-checkout.php`

Regras observadas:

- Carrinho atual e mantido em `$_SESSION["cart"]`.
- Um `cartGroup` e criado com `md5(uniqid(rand(), true))`.
- Ao adicionar produto, verifica estoque disponivel considerando quantidade ja existente no carrinho.
- Atualizar quantidade so e permitido se estoque atual for suficiente.
- Checkout exige usuario autenticado.
- Checkout monta `Order` com itens, endereco, CPF e dados basicos de pagamento.
- Se pedido falhar parcialmente, pedido criado e removido.
- Apos sucesso, estoque dos produtos e reduzido e carrinho e limpo.
- Frete atual e valor fixo no model de carrinho.

### Pedidos

Responsabilidades:

- Listar compras do usuario autenticado.
- Exibir detalhes de pedido.
- Exibir itens comprados, vendedor, quantidade, preco e subtotal.

Rotas principais:

- `/admin/usuario/minhas-compras`
- `/admin/pedido/detalhes`

Views:

- `views/admin/users/lista-compras.php`
- `views/admin/users/lista-compras-table.php`
- `views/admin/order/details.php`

Regras observadas:

- Lista de pedidos e filtrada por `userId`.
- Detalhe do pedido inclui dados de entrega e produtos.

### Banco, seed e utilitarios

Responsabilidades:

- Criar banco.
- Destruir banco.
- Executar seed.
- Logging simples em arquivo.

Rotas principais:

- `/createdb`
- `/destroydb`
- `/seed`

Observacao:

- Essas rotas estao publicas no legado. Na migracao devem ser removidas da superficie publica ou protegidas por ambiente/permissao administrativa.

## Entidades e relacionamentos

Entidades/tabelas identificadas no SQL:

- `users`: usuario, login, senha, nome, role, CPF e sobrenome.
- `sellers`: dados complementares de vendedor, vinculado a `users`.
- `addresses`: endereco de usuario, vinculado a `users` e `states`.
- `states`: estados.
- `categories`: categorias com imagem.
- `subcategories`: subcategorias vinculadas a categorias.
- `products`: produtos vinculados a usuario/vendedor e subcategoria.
- `productsimages`: imagens de produto.
- `attributes`: nomes de atributos.
- `attributevalues`: valores de atributos por produto.
- `similarproducts`: associacao produto pai/produto filho.
- `productslikeds`: curtidas de produto por usuario.
- `ratings`: avaliacoes de produto por usuario, com aprovacao.
- `orders`: pedido com total, usuario, estado e dados de entrega/pagamento.
- `orderitens`: itens de pedido.
- `carouselimages`: imagens do carrossel.

Relacionamentos principais:

- `users` 1:N `addresses`
- `users` 1:0..1 `sellers`
- `users` 1:N `products`
- `users` 1:N `orders`
- `states` 1:N `addresses`
- `states` 1:N `orders`
- `categories` 1:N `subcategories`
- `subcategories` 1:N `products`
- `products` 1:N `productsimages`
- `products` 1:N `attributevalues`
- `attributes` 1:N `attributevalues`
- `products` N:N `products` via `similarproducts`
- `users` N:N `products` via `productslikeds`
- `products` 1:N `ratings`
- `orders` 1:N `orderitens`
- `products` 1:N `orderitens`

## Integracoes e dependencias

Dependencias externas/front-end:

- jQuery.
- Dropzone 5.5.1 para upload de imagens.
- SweetAlert2 8.18.5 para mensagens e confirmacoes.
- FontAwesome.
- Slick/tiny-slider/sidebar scripts.

Persistencia legado:

- MySQL via PDO.
- SQL inicial em `script-database-generate.sql`.

Arquivos/imagens:

- Uploads e assets em `assets/img/products`, `assets/img/categories`, `assets/img/carousel` e `assets/img/destaques`.
- Na migracao, esses arquivos devem virar assets estaticos ou objetos em storage/local file provider, mantendo nomes para compatibilidade de dados.

Integracoes externas reais:

- Nao foram identificadas APIs externas.
- Pagamento e frete sao simulados; dados de cartao sao persistidos parcialmente no pedido no legado, mas isso deve ser remodelado para nao armazenar dados sensiveis indevidos.

## Riscos e inconsistencias a tratar na migracao

- Rotas `/createdb`, `/destroydb` e `/seed` estao publicas.
- Carrinho vive em sessao PHP; em .NET deve ser repensado como sessao distribuivel, cache ou persistencia por usuario/visitante.
- Dados de pagamento sao tratados de forma simplificada; nao migrar armazenamento de dados sensiveis de cartao.
- Algumas consultas SQL parecem ter erros ou fragilidades, como ausencia de `execute()` em alguns totais e trechos com aliases/virgulas inconsistentes.
- Exclusoes fazem cascatas manuais e podem deixar inconsistencias se uma etapa falhar.
- Nomes de tabela e coluna variam em caixa e pluralizacao.
- Validacoes sao incompletas e espalhadas em models/services.
- Controllers e services acessam `$_POST`, `$_GET` e `$_SESSION` diretamente.
- Uploads usam nomes de arquivos em string; precisa politica clara de armazenamento, sanitizacao e limite de tamanho.
- Autorizacao por string de role deve ser migrada para policies/claims.
- Alguns controllers retornam HTML parcial, outros JSON, e outros redirecionam; isso precisa ser explicitado por slice.

## Plataforma alvo proposta

Backend:

- .NET 10.
- ASP.NET Core.
- Minimal APIs por feature.
- Vertical Slice Architecture.
- Entity Framework Core.
- SQL Server como banco alvo.
- ASP.NET Identity para usuarios.
- JWT Authentication para API.
- FluentValidation para requests.
- ProblemDetails para erros.
- Logging estruturado.

Frontend:

- Blazor.
- Componentes pequenos por fluxo.
- UI proxima ao legado, principalmente area admin, catalogo, detalhes de produto, carrinho e checkout.

Arquitetura sugerida:

```text
src/
  Marketplace.Api/
    Features/
      Auth/
      Users/
      Sellers/
      Catalog/
      Products/
      Categories/
      SubCategories/
      Attributes/
      Cart/
      Checkout/
      Orders/
      Ratings/
      Likes/
      SimilarProducts/
      Files/
    Infrastructure/
      Persistence/
      Identity/
      Files/
      Errors/
  Marketplace.Web/
    Features/
      Catalog/
      Account/
      Admin/
      Cart/
      Checkout/
  Marketplace.Tests/
```

## Plano de migracao em etapas

### Etapa 1 - Fundacao tecnica

Criar solution .NET, projetos API/Web/Tests, configuracao de DI, EF Core, Identity, ProblemDetails, logging, middleware global de erros, CORS quando necessario e padrao de slices.

Entregaveis:

- Solution compilando.
- Health check.
- DbContext configurado.
- Identity configurado.
- Estrutura vertical por features.

### Etapa 2 - Modelo de dominio e banco

Modelar entidades principais e migrations SQL Server a partir do SQL MySQL legado, corrigindo inconsistencias estruturais com cautela.

Prioridade:

1. Users/Identity.
2. Sellers.
3. Addresses/States.
4. Categories/SubCategories.
5. Products/ProductImages/Attributes/AttributeValues.
6. Cart/Orders/OrderItems.
7. Ratings/Likes/SimilarProducts/CarouselImages.

Entregaveis:

- Migrations.
- Configuracoes Fluent API.
- Conversao do schema MySQL legado para modelo EF Core/SQL Server.
- Seed inicial equivalente ao SQL legado quando util, com dados sensiveis revisados.

### Etapa 3 - Autenticacao, usuarios e autorizacao

Migrar login, registro comum, registro de vendedor, logout, roles e protecao de rotas.

Slices sugeridos:

- `Auth/Login`
- `Auth/RegisterUser`
- `Auth/RegisterSeller`
- `Users/GetCurrent`
- `Users/List`
- `Users/Create`
- `Users/Update`
- `Users/Delete`
- `Users/Addresses/Update`

Validacoes:

- Login unico.
- Senha obrigatoria.
- Nome/login obrigatorios.
- Apenas admin altera role.

### Etapa 4 - Catalogo base

Migrar listagem publica de home, categorias, subcategorias, pesquisa e detalhes de produto.

Slices sugeridos:

- `Catalog/Home`
- `Catalog/SearchProducts`
- `Catalog/GetProductDetails`
- `Catalog/ListByCategory`
- `Catalog/ListBySubCategory`
- `Catalog/ListCarousel`

Entregaveis:

- UI Blazor publica.
- APIs REST com DTOs.
- Paginacao equivalente.

### Etapa 5 - Administracao de catalogo

Migrar CRUD de categorias, subcategorias e atributos.

Slices sugeridos:

- `Categories/Create/Update/Delete/List/GetJson`
- `SubCategories/Create/Update/Delete/List/ListByCategory`
- `Attributes/Create/Update/Delete/List`

Entregaveis:

- Telas admin Blazor.
- Upload de imagem de categoria.
- Validacoes com FluentValidation.

### Etapa 6 - Produtos

Migrar CRUD de produtos, upload de imagens, ficha tecnica, produto em oferta, estoque, SKU e regra de escopo por vendedor.

Slices sugeridos:

- `Products/ListAdmin`
- `Products/Create`
- `Products/Update`
- `Products/Delete`
- `Products/UploadImage`
- `Products/AddAttributeValue`
- `Products/UpdateAttributeValues`

Regras:

- Admin pode escolher vendedor.
- Vendedor gerencia apenas seus produtos.
- Edicao deve substituir imagens/atributos conforme comportamento legado, ou adotar endpoints explicitos de adicionar/remover se documentado.

### Etapa 7 - Carrinho e checkout

Migrar carrinho, atualizacao de quantidade, remocao, checkout e baixa de estoque.

Slices sugeridos:

- `Cart/Get`
- `Cart/AddProduct`
- `Cart/RemoveProduct`
- `Cart/UpdateQuantity`
- `Checkout/Get`
- `Checkout/Submit`

Decisao tecnica:

- Carrinho sera persistido em banco.
- Visitantes devem receber um identificador anonimo de carrinho.
- Ao autenticar, o carrinho anonimo deve ser associado ou mesclado ao usuario autenticado.
- Estoque deve ser validado ao adicionar, atualizar quantidade e finalizar checkout.
- Carrinhos abandonados devem poder ser expirados por rotina futura, sem bloquear a primeira migracao.

### Etapa 8 - Pedidos

Migrar lista de compras e detalhes do pedido.

Slices sugeridos:

- `Orders/ListMine`
- `Orders/GetDetails`

Regras:

- Usuario ve seus pedidos.
- Admin pode acessar conforme regra desejada.
- Detalhe inclui entrega e itens.

### Etapa 9 - Recursos sociais e recomendacao

Migrar curtidas, produtos curtidos, avaliacoes, aprovacao de avaliacoes e produtos similares.

Slices sugeridos:

- `Likes/LikeProduct`
- `Likes/UnlikeProduct`
- `Likes/ListMine`
- `Ratings/Create`
- `Ratings/ListPending`
- `Ratings/Approve`
- `SimilarProducts/List`
- `SimilarProducts/Add`
- `SimilarProducts/Delete`
- `SimilarProducts/ListChoices`

### Etapa 10 - UI Blazor completa

Migrar telas mantendo experiencia semelhante ao legado:

- Layout publico.
- Layout admin.
- Navegacao por role.
- Formularios com validacao.
- Componentes de tabela paginada.
- Componentes de upload.
- Componentes de produto/card.
- Checkout.

### Etapa 11 - Testes e compatibilidade funcional

Criar testes focados nos fluxos criticos:

- Registro/login.
- Autorizacao por role.
- Produto CRUD.
- Carrinho com validacao de estoque.
- Checkout com baixa de estoque.
- Pedido e detalhes.
- Curtida idempotente.
- Avaliacao pendente/aprovacao.

### Etapa 12 - Hardening e encerramento

Remover rotas perigosas do legado equivalente, revisar logs, erros padronizados, seguranca de upload, armazenamento de dados sensiveis, performance de consultas e documentacao da migracao.

## Ordem recomendada de execucao

1. Fundacao tecnica.
2. Banco e entidades.
3. Identity/autenticacao/roles.
4. Usuarios e vendedores.
5. Categorias/subcategorias/atributos.
6. Catalogo publico.
7. Produtos admin.
8. Carrinho.
9. Checkout/pedidos.
10. Curtidas/avaliacoes/similares.
11. Blazor refinado e compatibilidade visual.
12. Testes, ajustes e limpeza final.

## Observacao final

Decisoes arquiteturais registradas:

- Banco alvo: SQL Server.
- Estrategia de carrinho: persistido em banco, com suporte a carrinho anonimo e associacao/mesclagem apos login.
