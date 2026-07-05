<?php require_once './views/partials/header-admin.php' ?>
<?php

use infra\helpers\SrcHelper;
?>
<link rel="stylesheet" href="/assets/libs/dropzone/dropzone.min.css">
<div class="container">
    <div class="d-flex align-items-center p-3 mt-3 text-white-50 bg-dark rounded shadow-sm">
        <div class="lh-100">
            <h6 class="mb-0 text-white lh-100">Cadastro de categoria</h6>
            <small>Categorias > Cadastrar</small>
        </div>
    </div>

    <div class="card mt-3">
        <div class="card-body">
            <form action="/admin/categoria/cadastrar-post" method="post" id="formCategories" data-img-path="<?php echo SrcHelper::getCategoryImg(); ?>">
                <h5>Informe os dados da categoria e clique em salvar.</h6>
                    <input type="hidden" id="images" name="images" value="">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-group">
                                <label for="title">Titulo:</label>
                                <input type="text" name="title" id="title" data-required="true" class="form-control" placeholder="" aria-describedby="helpId">
                                <small id="help-title" class="text-muted">Nome da categoria</small>
                            </div>
                        </div>
                        <!--imagens atuais e container de uploads-->
                        <div class="col-md-12">
                            <div class="form-group">
                                <label for="">Imagens atuais:</label>
                                <div id="categories-img-card-container" class="card-columns ">
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12">
                            <div class="form-group">
                                <label for="">Selecione uma ou mais imagens para seu produto:</label>
                                <div id="upload-container" class="dropzone">
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12">
                            <a class="btn btn-sm btn-warning" href="/admin/categoria">
                                <i class="fa fa-chevron-left"></i>
                            </a>
                            <button type="submit" name="btn-salvar" id="btn-salvar" class="btn btn-sm btn-dark"><i class="fa fa-save"></i> Salvar categoria
                            </button>
                        </div>
                    </div>
            </form>
        </div>
    </div>
</div>

<?php require_once './views/partials/scripts-admin.php' ?>
<script src="/assets/libs/dropzone/dropzone.min.js"></script>
<script src="/assets/js/models/CategoryImageCard.js"></script>
<script src="/assets/js/models/CategoryDropzone.js"></script>
<script src="/assets/js/models/CategoryForm.js"></script>
<?php require_once './views/partials/footer-admin.php' ?>