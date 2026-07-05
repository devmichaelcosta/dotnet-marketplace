<?php
require_once './views/partials/header-admin.php';
?>
<div class="container">
    <div class="d-flex align-items-center p-3 mt-3 text-white-50 bg-dark rounded shadow-sm">
        <div class="lh-100">
            <h6 class="mb-0 text-white lh-100">Lista de atributos</h6>
            <ul class="nav-breadcrumb">
                <li>
                    <a href="/admin/atributo">Atributos</a>
                </li>
                <li>
                    <a href="javascript:void(0);">Lista de atributos</a>
                </li>
            </ul>
        </div>
    </div>

    <div class="card mt-3 ">
        <div class="card-body">
            <h5>Veja abaixo as categorias disponiveis.</h6>
            <div class="row">
                <div class="col-md-12">
                    <div class="form-group">
                        <div class="input-group mb-3">
                            <input type="text" id="search-attributes" name="search-attributes" class="form-control" placeholder="Pesquise pelo nome do atributo" aria-label="Pesquise pelo nome do atributo" aria-describedby="btn-pesquisar">
                            <div class="input-group-append">
                                <button class="btn btn-outline-secondary" type="button" id="btn-pesquisar">
                                    Pesquisar
                                </button>
                            </div>
                        </div>
                    </div>
                    <p>
                        <a class="btn btn-dark btn-sm" href="/admin/atributo/cadastrar">
                            <i class="fa fa-plus"></i> Cadastrar atributo
                        </a>
                    </p>
                </div>
            </div>
            <div id="list-container">
                <?php include './views/admin/attributes/list.php' ?>
            </div>
            <div class="row">
                <div class="col-md-6">
                    <p>
                        <a class="btn btn-dark btn-sm" href="/admin/atributo/cadastrar">
                            <i class="fa fa-plus"></i> Cadastrar atributo
                        </a>
                    </p>
                </div>
            </div>
        </div>
    </div>
</div>


<?php require_once './views/partials/scripts-admin.php' ?>
<script src="/assets/js/admin/attribute/AttributeList.js"></script>
<?php require_once './views/partials/footer-admin.php' ?>