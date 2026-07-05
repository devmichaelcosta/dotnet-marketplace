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
                            <li class="breadcrumb-item"><a href="#">Categoria</a></li>
                            <li class="breadcrumb-item"><a href="#"><?php echo $category->getTitle(); ?></a></li>
                            <li class="breadcrumb-item active" aria-current="page"></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </section>
        <section>
            <div class="row">
                <div class="col-md-3">
                    <div class="card p-2 categories-menu">
                        <span><?php echo $category->getTitle(); ?></span>
                        <ul class="list-unstyled">
                            <?php foreach ($category->getSubCategories() as $subCategory): ?>
                            <li>
                                <a href="javascript:void(0)" data-id="<?php echo $subCategory->getSubCategoryId(); ?>"><?php echo $subCategory->getTitle(); ?></a>
                            </li>
                            <?php endforeach; ?>
                        </ul>
                    </div>
                </div>
                <div class="col-md-9">
                    <div class="card bg-light">
                        <div class="card-header">
                            <span id="qtd-produtos">0</span> produtos encontrados
                        </div>
                    </div>      
                    <div id="products-list" class="mt-2"></div>
                </div>
            </div>
        </section>
    </div>
</main>

<?php require_once './views/partials/footer.php' ?>
<script src="/assets/js/web/product/list-by-category.js"></script>