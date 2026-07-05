<?php require_once './views/partials/header-admin.php' ?>

<div class="container">
    <div class="d-flex align-items-center p-3 mt-3 text-white-50 bg-dark rounded shadow-sm">
        <div class="lh-100">
            <h6 class="mb-0 text-white lh-100">Detalhes do vendedor</h6>
            <ul class="nav-breadcrumb">
                <li>
                    <a href="/admin/vendedor">Vendedores</a>
                </li>
                <li>
                    <a href="/admin/vendedor/detalhes?id=<?php echo $model->getSellerId(); ?>">Detalhes</a>
                </li>
            </ul>
        </div>
    </div>

    <div class="card mt-3  mb-3">
        <div class="card-body">
            <div class="row">
                <input type="hidden" name="sellerId" id="sellerId" value="<?php echo $model->getSellerId(); ?>">
                <div class="col-md-12 mb-2" id="container-dados-gerais">
                    <h5>Dados gerais do vendedor:</h5>
                    <div class="container-gray custom-labels">
                        <div class="row">
                            <div class="col-md-6">
                                <label for="name">Nome:</label> <?php echo $model->getName(); ?><br>
                                <label for="lastName">Sobrenome:</label><?php echo $model->getLastName(); ?><br>
                                <label for="email">E-mail:</label><?php echo $model->getEmail(); ?><br>
                                <label for="website">Site:</label><?php echo $model->getWebsite(); ?>
                            </div>
                            <div class="col-md-6">

                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12">

                        <div class="card-body">
                            <h5>Produtos do vendedor:</h5>
                            <div class="row">
                                <?php foreach ($model->getProducts() as $product) : ?>
                                    <div class="col-md-4 ">
                                        <div class="card h-100">
                                            <div style="text-align:center; ">
                                                <img src="/img/products/<?php echo $product->getDefaultImage(); ?>" class="card-img-top " style="max-width:120px; max-height:120px" alt="...">
                                            </div>
                                            <div class="card-body ">
                                                <p class="card-text text-center">
                                                    <?php echo $product->getTitle(); ?>
                                                </p>
                                                <a href="/admin/produto/editar?id=<?php echo $product->getId(); ?>" class="btn btn-dark btn-block">
                                                    Editar
                                                </a>
                                            </div>
                                        </div>
                                    </div>
                                <?php endforeach; ?>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-md-12">
                    <a class="btn btn-sm btn-warning" href="/admin/vendedor">
                        <i class="fa fa-chevron-left"></i>
                    </a>
                    <a class="btn btn-sm btn-dark" href="/admin/vendedor/editar?id=<?php echo $model->getSellerId(); ?>">
                        <i class="fa fa-edit"></i> Editar vendedor
                    </a>
                </div>
            </div>

        </div>
    </div>
</div>

<?php require_once './views/partials/scripts-admin.php' ?>
<!-- <script src="/assets/libs/jquery.mask/jquery.mask.min.js"></script>
<script src="/assets/js/models/SellerEdit.js"></script> -->
<?php require_once './views/partials/footer-admin.php' ?>