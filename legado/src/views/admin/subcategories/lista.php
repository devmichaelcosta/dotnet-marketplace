<?php
require_once './views/partials/header-admin.php';
?>
<div class="container">
    <div class="d-flex align-items-center p-3 mt-3 text-white-50 bg-dark rounded shadow-sm">
        <div class="lh-100">
            <h6 class="mb-0 text-white lh-100">Lista de subcategorias</h6>
            <ul class="nav-breadcrumb">
                <li>
                    <a href="/admin/categoria">Categorias</a>
                </li>
                <li>
                    <a href="/admin/subcategoria?id=<?php echo $_GET["id"]; ?>">Subcategorias</a>
                </li>
                <li>Lista de subcategorias para <?php echo $category->getTitle(); ?></li>
            </ul>
        </div>
    </div>

    <div class="card mt-3 ">
        <div class="card-body">
            <h5>Veja abaixo as subcategorias disponiveis para <?php echo $category->getTitle(); ?>.</h6>
            <div class="row">
                <div class="col-md-12">
                    <div class="form-group">
                        <div class="input-group mb-3">
                            <input type="text" id="search" name="search" class="form-control" 
                                placeholder="Pesquise pelo título da subcategoria" aria-label="Pesquise pelo titulo da subcategoria" 
                                aria-describedby="btn-pesquisar">
                            <div class="input-group-append">
                                <button class="btn btn-outline-secondary" type="button" id="btn-pesquisar">
                                    Pesquisar
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <?php include './views/admin/subcategories/buttons-in-list.php' ?>
            <div id="list-container" data-category-id="<?php echo $_GET["id"]; ?>">
                <?php include './views/admin/subcategories/lista-table.php' ?>
            </div>
            <?php include './views/admin/subcategories/buttons-in-list.php' ?>
        </div>
    </div>
</div>

<?php require_once './views/partials/scripts-admin.php' ?>
<script src="/assets/js/models/SubCategoryList.js"></script>
<?php require_once './views/partials/footer-admin.php' ?>