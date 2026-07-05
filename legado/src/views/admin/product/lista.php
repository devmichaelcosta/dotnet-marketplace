<?php
require_once './views/partials/header-admin.php';
?>
<div class="container">
    <div class="d-flex align-items-center p-3 mt-3 text-white-50 bg-dark rounded shadow-sm">
        <div class="lh-100">
            <h6 class="mb-0 text-white lh-100">Lista de produtos</h6>
            <ul class="nav-breadcrumb">
                <li>
                    <a href="/admin/produto">Produtos</a>
                </li>
                <li>
                    <a href="javascript:void(0);">Lista de produtos</a>
                </li>
            </ul>
        </div>
    </div>

    <div class="card mt-3 ">
        <div class="card-body">
            <h5>Veja abaixo os produtos disponiveis.</h6>
                <!-- pesquisa de produtos -->
                <div class="row">
                    <div class="col-md-12">
                        <div class="form-group">
                            <div class="input-group mb-3">
                                <input type="text" class="form-control" id="search-products" placeholder="Pesquise pelo titulo do produto" aria-label="Pesquise pelo titulo do produto" aria-describedby="btn-pesquisar">
                                <div class="input-group-append">
                                    <button class="btn btn-outline-secondary" type="button" id="btn-pesquisar">
                                        Pesquisar
                                    </button>
                                </div>
                            </div>
                        </div>
                        <p>
                            <a class="btn btn-dark btn-sm " href="/admin/produto/cadastrar">
                                <i class="fa fa-plus"></i> Cadastrar produto
                            </a>
                        </p>
                    </div>
                </div>
                <!-- lista de produtos -->
                <div id="container-products">
                    <?php include './views/admin/product/lista-table.php' ?>
                </div>
                <p class="mt-3">
                    <a class="btn btn-dark btn-sm" href="/admin/produto/cadastrar">
                        <i class="fa fa-plus"></i> Cadastrar produto
                    </a>
                </p>
        </div>
    </div>
</div>
<?php require_once './views/partials/scripts-admin.php' ?>
<script src="/assets/js/admin/product/ListOfProducts.js"></script>
<?php require_once './views/partials/footer-admin.php' ?>