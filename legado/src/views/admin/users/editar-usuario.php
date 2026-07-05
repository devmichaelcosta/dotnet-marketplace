<?php require_once './views/partials/header-admin.php' ?>

<div class="container">
    <div class="d-flex align-items-center p-3 mt-3 text-white-50 bg-dark rounded shadow-sm">
        <div class="lh-100">
            <h6 class="mb-0 text-white lh-100">Edição de usuário</h6>
            <ul class="nav-breadcrumb">
                <li>
                    <a href="/admin/usuario">Lista de usuários</a>
                </li>
                <li>
                    <a href="/admin/usuario/editar?id=<?php echo $user->getUserId(); ?>">Editar usuário</a>
                </li>
            </ul>
        </div>
    </div>
    <form action="/admin/usuario/editar-post" method="post" id="formUsuario">
        <div class="card mt-3">
            <div class="card-body">
                <h5>Informe os dados do usuario e clique em salvar.</h6>
                    <div class="row">

                        <input type="hidden" name="userId" id="userId" value="<?php echo $user->getUserId(); ?>">

                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="name">Nome:</label>
                                <input type="text" name="name" id="name" data-required="true" value="<?php echo $user->getName(); ?>" class="form-control" placeholder="" aria-describedby="helpId">
                                <small id="help-name" class="text-muted">Nome do usuário</small>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="login">Login:</label>
                                <input type="text" name="login" id="login" data-required="true" value="<?php echo $user->getLogin(); ?>" class="form-control" placeholder="" aria-describedby="helpId">
                                <small id="help-login" class="text-muted">Login do usuário</small>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="">SobreNome:</label>
                                <input type="text" data-required="true" class="form-control" name="lastName" id="lastName" value="<?php echo $user->getLastName(); ?>" aria-describedby="lastNameHelp" placeholder="">
                                <small id="lastNameHelp" class="form-text text-muted">
                                    Seu sobrenome.
                                </small>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="">Cpf:</label>
                                <input type="text" data-required="true" class="form-control" name="cpf" id="cpf" value="<?php echo $user->getCpf(); ?>" aria-describedby="cpfId" placeholder="">
                                <small id="cpfId" class="form-text text-muted">
                                    Seu cpf.
                                </small>
                            </div>
                        </div>

                        <?php if ($_SESSION["role"] == "admin") : ?>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="role">Tipo:</label>
                                    <select name="role" id="role" data-required="true" class="form-control" aria-describedby="help-role">
                                        <option value="">Selecione o tipo...</option>
                                        <option value="comum" <?php echo $user->getRole()  == "comum" ? "selected=selected" : ""; ?>>
                                            Comum
                                        </option>
                                        <option value="vendedor" <?php echo $user->getRole() == "vendedor" ? "selected=selected" : ""; ?>>
                                            Vendedor
                                        </option>
                                        <option value="admin" <?php echo $user->getRole() == "admin" ? "selected=selected" : ""; ?>>
                                            Administrador
                                        </option>
                                    </select>
                                    <small id="help-role" class="text-muted">Tipo de usuário</small>
                                </div>
                            </div>
                        <?php endif; ?>


                    </div>

            </div>
        </div>

        <div class="card mt-3 mb-3">
            <div class="card-body">
                <div class="row mb-3">
                    <div class="col-md-6">
                        <h5 class="title">Dados de entrega:</h5>
                    </div>
                    <div class="col-md-6 text-right">

                    </div>
                </div>
                <div id="addresses-container">
                    <?php foreach ($userAddresses as $address) : ?>
                        <?php require './views/admin/users/address.php' ?>
                    <?php endforeach; ?>
                </div>
                <div class="row">
                    <div class="col-md-12 mt-3">
                        <a class="btn btn-warning btn-sm " href="/admin/usuario">
                            <i class="fa fa-chevron-left"></i>
                        </a>
                        <button type="button" class="btn btn-sm btn-dark" id="btn-add-address" data-toggle="button" aria-pressed="false" autocomplete="off">
                            <i class="fa fa-plus"></i> Adicionar no endereço
                        </button>
                        <button type="submit" name="btn-salvar" id="btn-salvar" class="btn btn-dark btn-sm "><i class="fa fa-save"></i> Salvar usuário
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </form>
</div>

<?php require_once './views/partials/scripts-admin.php' ?>
<script src="/assets/libs/jquery.mask/jquery.mask.min.js"></script>
<script src="/assets/js/models/UserForm.js"></script>
<?php require_once './views/partials/footer-admin.php' ?>