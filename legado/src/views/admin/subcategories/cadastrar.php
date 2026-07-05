<?php require_once './views/partials/header-admin.php' ?>
<?php
    use infra\helpers\SrcHelper;
?>
<div class="container">
    <div class="d-flex align-items-center p-3 mt-3 text-white-50 bg-dark rounded shadow-sm">
        <div class="lh-100">
            <h6 class="mb-0 text-white lh-100">Cadastro de sub categoria</h6>
            <small>Sub Categorias > Cadastrar</small>
        </div>
    </div>

    <div class="card mt-3">
        <div class="card-body">
            <form action="/admin/subcategoria/cadastrar-post" method="post" id="form-cadastro" data-img-path="<?php echo SrcHelper::getCategoryImg(); ?>">
                <h5>Informe os dados da sub categoria e clique em salvar.</h6>
                <input type="hidden" id="categoryId" name="categoryId" value="<?php echo $_GET["id"]; ?>">
                <div class="row">
                    <div class="col-md-12">
                        <div class="form-group">
                            <label for="title">Nome:</label>
                            <input type="text" name="title" id="title" data-required="true" class="form-control" placeholder="" aria-describedby="helpId">
                            <small id="help-title" class="text-muted">Nome da sub categoria</small>
                        </div>
                    </div>
                   
                    <div class="col-md-12">
                        <a class="btn btn-sm btn-warning" href="/admin/subcategoria?id=<?php echo $_GET["id"]; ?>">
                            <i class="fa fa-chevron-left"></i>
                        </a>
                        <button type="submit" name="btn-salvar" id="btn-salvar" class="btn btn-sm btn-dark">
                            <i class="fa fa-save"></i> Salvar
                        </button>
                    </div>
                </div>
            </form>
        </div>
    </div>
</div>

<?php require_once './views/partials/scripts-admin.php' ?>
<script src="/assets/js/models/SubCategoryForm.js"></script>
<?php require_once './views/partials/footer-admin.php' ?>