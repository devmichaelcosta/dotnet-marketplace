<?php 
     require_once './views/partials/header-admin.php';
?>

<div class="container">
    <div class="d-flex align-items-center p-3 mt-3 text-white-50 bg-dark rounded shadow-sm">
        <div class="lh-100">
            <h6 class="mb-0 text-white lh-100">Lista de compras</h6>
            
            <ul class="nav-breadcrumb">
                <li>
                    <a href="/admin/usuario/minhas-compras">Compras</a>
                </li>
                <li>
                    <a href="javascript:void(0)">Lista de compras</a>
                </li>
            </ul>
        </div>
    </div>

    <div class="card mt-3 ">
        <div class="card-body">
            <h5>Veja abaixo as compras disponiveis.</h6>
            <div class="row">
                <div class="col-md-12">
                    <div class="form-group">
                        <div class="input-group mb-3">
                            <input type="text" class="form-control" id="search-users" 
                                placeholder="Pesquise pelo titulo da categoria" 
                                aria-label="Pesquise pelo titulo da categoria" 
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
            <div class="row">
                <div class="col-md-12">
                    <?php include './views/admin/users/lista-compras-table.php' ?>
                </div>
            </div>
           
           
        </div>
    </div>
</div>

<?php require_once './views/partials/scripts-admin.php' ?>
<?php require_once './views/partials/footer-admin.php' ?>