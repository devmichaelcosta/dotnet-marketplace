<?php require_once './views/partials/header-admin.php' ?>
<div class="container">
    <div class="d-flex align-items-center p-3 mt-3 text-white-50 bg-dark rounded shadow-sm">
        <div class="lh-100">
            <h6 class="mb-0 text-white lh-100">Cadastro de atributo</h6>
            <small>Atributos > Cadastrar</small>
        </div>
    </div>

    <div class="card mt-3">
        <div class="card-body">
            <form action="/admin/atributo/cadastrar-post" method="post" id="formAttributes">
                <h5>Informe os dados da atributo e clique em salvar.</h5>
                <div class="row">
                    <div class="col-md-12">
                        <div class="form-group">
                            <label for="name">Nome:</label>
                            <input type="text" name="name" id="name" data-required="true" class="form-control" placeholder="" aria-describedby="helpId">
                            <small id="help-title" class="text-muted">Nome do atributo</small>
                        </div>
                    </div>

                    <div class="col-md-12">
                        <a class="btn btn-sm btn-warning" href="/admin/categoria">
                            <i class="fa fa-chevron-left"></i>
                        </a>
                        <button type="submit" name="btn-salvar" id="btn-salvar" class="btn btn-sm btn-dark"><i class="fa fa-save"></i> Salvar atributo
                        </button>
                    </div>
                </div>
            </form>
        </div>
    </div>
</div>

<?php require_once './views/partials/scripts-admin.php' ?>
<script src="/assets/js/admin/attribute/AttributeForm.js"></script>
<?php require_once './views/partials/footer-admin.php' ?>