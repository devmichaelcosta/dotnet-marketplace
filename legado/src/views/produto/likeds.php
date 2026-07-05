<?php
    $titlePage = "Loja Whatever - Sua loja de instrumentos músicais e acessórios";
    require_once './views/partials/header.php';
?>
<style>
    .categories-menu.card{
        min-height:400px;
    }
</style>
<!-- conteudo principal -->
<main>
    <div id="container-products-category" class="container">
        <section id="caminho-produto">
            <div class="row mt-3">
                <div class="col-md-12">
                    <nav aria-label="breadcrumb">
                        <ol class="breadcrumb bg-dark">
                            <li class="breadcrumb-item"><a href="#">Produtos</a></li>
                            <li class="breadcrumb-item"><a href="#">Curtidos</a></li>
                            <li class="breadcrumb-item active" aria-current="page"></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </section>
        <section>
            <div class="row">
                <div class="col-md-12">
                    <div class="card bg-light">
                        <div class="card-header">
                            <span id="qtd-produtos"><?php echo count($products); ?></span> 
                            produtos encontrados
                        </div>
                    </div>      
                    <div id="products-list" class="mt-2">
                        <div class="row">
                            <?php foreach ($products as $product): ?>
                            <div class="col-lg-4 col-md-4 col-sm-6 mb-3 col-xs-12">
                                <div class="card card-oferta fade h-100">
                                    <div class="p-3 text-center">
                                        <img src="<?php echo $product->getDefaultImage(); ?>" class="card-img-top" >
                                    </div>
                                    <div class="card-body">
                                        <h5 class="card-title text-center">
                                        <?php echo $product->getTitle(); ?>
                                        </h5>
                                        <p class="card-price h3 text-center">
                                            R$ <?php echo $product->getPrice(); ?>
                                        </p>
                                        <a href="/detalhes-produto?id=<?php echo $product->getId(); ?>" 
                                            class="btn btn-block btn-outline-dark"> Ver detalhes
                                        </a>
                                    </div>
                                </div>
                            </div>
                            <?php endforeach?>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>
</main>

<?php require_once './views/partials/footer.php' ?>
<!-- <script src="/assets/js/web/product/list-by-category.js"></script> -->