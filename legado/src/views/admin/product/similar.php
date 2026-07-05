<?php
require_once './views/partials/header-admin.php';
?>
<div class="container">
    <input type="hidden" name="productId" id="productId" value="<?php echo $model->getId(); ?>">
    <div class="d-flex align-items-center p-3 mt-3 text-white-50 bg-dark rounded shadow-sm">
        <div class="lh-100">
            <h6 class="mb-0 text-white lh-100">Produtos similares</h6>
            <ul class="nav-breadcrumb">
                <li>
                    <a href="/admin/produto">Produtos</a>
                </li>
                <li>
                    <a href="javascript:void(0);">Produtos similares</a>
                </li>
            </ul>
        </div>
    </div>

    <div class="card mt-3 ">
        <div class="card-body">
            <!-- pesquisa de produtos -->
            <div class="row mb-4">
                <div class="col-md-4">
                    <div class="p-3 product-similar details-current-product">
                        <h6>
                            Veja abaixo os produtos semelhantes ao produto:<br /><br /> <strong><?php echo $model->getTitle(); ?></strong>.
                        </h6>
                        <div class="text-center pt-3">
                            <img src="<?php echo $model->getDefaultImage(); ?>" src="..." alt="..." class="img-fluid" style="max-height:100px;">
                        </div>
                    </div>
                </div>
                <div class="col-md-8">
                    <div class="form-group">
                        <div class="input-group mb-3">
                            <input type="text" class="form-control" id="search-products" placeholder="Pesquise pelo titulo do produto" aria-label="Pesquise pelo titulo do produto" aria-describedby="btn-pesquisar">
                            <div class="input-group-append">
                                <button class="btn btn-outline-secondary " type="button" id="btn-pesquisar">
                                    <i class="fa fa-search"></i>
                                </button>
                                <button class="btn btn-dark btn-sm" type="button" id="btn-add" data-product-id="<?php echo $model->getId(); ?>">
                                    Adicionar produto <i class="fa fa-plus"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                    <div id="container-products">
                        <?php include './views/admin/product/list-similar-products.php' ?>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<?php require_once './views/partials/scripts-admin.php' ?>
<script src="<?php echo $pathJs . "ListOfSimilarProducts.js" ?>"></script>
<?php require_once './views/partials/footer-admin.php' ?>