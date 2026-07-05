<?php
    $titlePage = "Loja Whatever - Sua loja de instrumentos músicais e acessórios";
    require_once './views/partials/header.php';
?>
<!-- conteudo principal -->
<main>
    <div class="container">
        <?php require_once './views/produto/breadcrumb.php' ?>

        <section id="detalhes-produto">
            <div class="row">
                <div class="col-sm-12 col-xs-12">
                    <div class="card p-3">
                        <div class="row">
                            <?php require_once './views/produto/details-product-images.php' ?>
                            
                            <div class="col-lg-7">
                                <div class="row">
                                    <div class="col-lg-10">
                                        <h4><?php echo $product->getTitle(); ?></h4>
                                    </div>
                                    <div class="col-lg-2 text-right">
                                    <?php if (isset($_SESSION["userId"])): ?>
                                        <i id="btn-like" data-product-id="<?php echo $product->getId(); ?>" class="cursor fa <?php echo ($isLiked === TRUE ? "fa-heart" : "fa-heart-o"); ?>"></i>
                                    <?php endif; ?>
                                    </div>
                                </div>
                                <div class="sku">(Cód. <?php echo $product->getSku(); ?>)</div>
                                <div class="vendedor">Vendido por: <strong><?php echo $product->getSeller(); ?></strong></div>
                                <div class="estoque">Qtd. estoque: <?php echo $product->getStock(); ?></div>
                                <?php if (intval($product->getStock()) > 0) : ?>
                                    <div class="preco">
                                        <div class="preco-avista h2 mb-0"><?php echo $product->getFormattedPrice() ?></div>
                                        <small class="parcela">em 1x no cartão</small>
                                    </div>
                                    <a class="btn btn-success btn-block" href="/adicionar-carrinho?id=<?php echo $product->getId(); ?>">
                                        <i class="fa fa-shopping-cart"></i> Adicionar no carrinho
                                    </a>
                                <?php endif; ?>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <section>
            <h3 class="mt-3">Informações do produto</h5>
            <div class="row">
                <div class="col-sm-12 col-xs-12">
                    <div class="card p-3">
                        <div class="row">
                            <div class="col-lg-12">
                                <p>
                                    <?php echo $product->getDescriptionFormatted(); ?>
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <?php require_once './views/produto/card-similar-product.php' ?>
        <?php require_once './views/produto/details-product-attributes.php' ?>
        <?php require_once './views/produto/details-product-comments.php' ?>
    </div>
</main>

<?php require_once './views/partials/footer.php' ?>
<script src="/assets/libs/slick/slick.min.js"></script>
<script src="/assets/js/web/product/details.js"></script>