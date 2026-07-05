<?php require_once './views/partials/header-admin.php' ?>

<div class="container">
    <div class="d-flex align-items-center p-3 mt-3 text-white-50 bg-dark rounded shadow-sm">
        <div class="lh-100">
            <h6 class="mb-0 text-white lh-100">Cadastro de vendedor</h6>
            <ul class="nav-breadcrumb">
                <li>
                    <a href="/admin/vendedor">Vendedores</a>
                </li>
                <li>
                    <a href="/admin/vendedor/cadastrar">Cadastrar</a>
                </li>
            </ul>
        </div>
    </div>

    <div class="card mt-3  mb-3">
        <div class="card-body">
            <form action="/admin/vendedor/cadastrar-post" method="post" id="form-cadastro">
                <h5>Informe os dados do usuario e clique em salvar.</h6>
                    <div class="row">
                        <div class="col-md-12">
                            <input type="radio" name="tipo-pessoa" id="tipo-fisica" value="fisica" checked="checked">
                            <label for="tipo-fisica"> Pessoa física</label>
                            <input type="radio" name="tipo-pessoa" id="tipo-juridica" value="juridica">
                            <label for="tipo-juridica"> Pessoa jurídica</label>
                        </div>
                        <div class="col-md-12 mb-2" id="container-dados-gerais">
                            <h5>Dados gerais.</h6>
                                <div class="container-gray">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label for="name">Nome:</label>
                                                <input type="text" name="name" id="name" data-required="true" class="form-control" placeholder="" aria-describedby="helpId">
                                                <small id="help-name" class="text-muted">Nome do usuário</small>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label for="lastName">Sobrenome:</label>
                                                <input type="text" name="lastName" id="lastName" data-required="true" class="form-control" placeholder="" aria-describedby="helpId">
                                                <small id="help-login" class="text-muted">Sobrenome</small>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label for="login">Login:</label>
                                                <input type="text" name="login" id="login" data-required="true" class="form-control" placeholder="" aria-describedby="helpId">
                                                <small id="help-login" class="text-muted">Login do usuário</small>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label for="password">Senha:</label>
                                                <input type="password" name="password" id="password" data-required="true" class="form-control" placeholder="" aria-describedby="helpId">
                                                <small id="help-password" class="text-muted">Senha do usuário</small>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label for="email">E-mail:</label>
                                                <input type="text" name="email" id="email" data-required="true" class="form-control" placeholder="" aria-describedby="help-email">
                                                <small id="help-email" class="text-muted">Seu e-mail</small>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label for="website">Site:</label>
                                                <input type="text" name="website" id="website" class="form-control" placeholder="" aria-describedby="help-website">
                                                <small id="help-website" class="text-muted">Seu website</small>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                        </div>
                        <div class="col-md-12 mb-2 " id="container-pessoa-fisica">
                            <h5>Dados para pessoa física.</h6>
                                <div class="container-gray">
                                    <div class="row">
                                        <div class="col-md-5">
                                            <div class="form-group">
                                                <label for="cpf">CPF:</label>
                                                <input type="text" name="cpf" id="cpf" class="form-control" data-required="true" placeholder="" aria-describedby="help-cpf" maxlength="14">
                                                <small id="help-cpf" class="text-muted">Seu cpf</small>
                                            </div>
                                        </div>
                                        <div class="col-md-3">
                                            <div class="form-group">
                                                <label for="age">Idade:</label>
                                                <input type="number" name="age" id="age" class="form-control" data-required="true" placeholder="" aria-describedby="help-age">
                                                <small id="help-age" class="text-muted">Sua idade</small>
                                            </div>
                                        </div>
                                        <div class="col-md-4">
                                            <div class="form-group">
                                                <label for="dataNascimento">Data de nascimento:</label>
                                                <input type="date" name="dataNascimento" id="dataNascimento" class="form-control" data-required="true" placeholder="" aria-describedby="help-dataNascimento">
                                                <small id="help-dataNascimento" class="text-muted">Sua data de nascimento</small>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                        </div>

                        <div class="col-md-12 mb-2 hidden" id="container-pessoa-juridica">
                            <h5>Dados para pessoa juridica.</h6>
                                <div class="container-gray">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label for="company">Empresa:</label>
                                                <input type="text" name="company" id="company" class="form-control" data-required="true" placeholder="" aria-describedby="help-company">
                                                <small id="help-company" class="text-muted">Sua empresa</small>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label for="nomeFantasia">Nome fantasia:</label>
                                                <input type="text" name="nomeFantasia" id="nomeFantasia" data-required="true" class="form-control" placeholder="" aria-describedby="help-nomeFantasia">
                                                <small id="help-nomeFantasia" class="text-muted">O nome fantasia da sua empresa</small>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label for="cnpj">CNPJ:</label>
                                                <input type="text" name="cnpj" id="cnpj" data-required="true" class="form-control" placeholder="" aria-describedby="help-cnpj">
                                                <small id="help-cnpj" class="text-muted">Seu cnpj</small>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label for="branchOfActivity">Ramo de atividade:</label>
                                                <input type="text" name="branchOfActivity" id="branchOfActivity" data-required="true" class="form-control" placeholder="" aria-describedby="help-branchOfActivity">
                                                <small id="help-branchOfActivity" class="text-muted">O ramo de atividade da empresa</small>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                        </div>

                        <div class="col-md-12 mb-2" id="container-local">
                            <h5>Dados do local.</h6>
                                <div class="container-gray">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label for="street">Rua:</label>
                                                <input type="text" name="street" id="street" class="form-control" placeholder="" data-required="true" aria-describedby="help-street">
                                                <small id="help-street" class="text-muted">Sua rua</small>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label for="neighborhood">Bairro:</label>
                                                <input type="text" name="neighborhood" id="neighborhood" class="form-control" placeholder="" data-required="true" aria-describedby="help-neighborhood">
                                                <small id="help-neighborhood" class="text-muted">Seu bairro</small>
                                            </div>
                                        </div>
                                        <div class="col-md-4">
                                            <div class="form-group">
                                                <label for="stateId">Estado:</label>
                                                <select data-required="true" class="form-control" name="stateId" id="stateId" data-required="true" aria-describedby="stateId" placeholder="">
                                                    <option value="">Selecione</option>
                                                    <?php foreach ($model->getStates() as $state) : ?>
                                                        <option value="<?php echo $state->getId(); ?>">
                                                            <?php echo $state->getName(); ?>
                                                        </option>
                                                    <?php endforeach; ?>
                                                </select>
                                                <small id="help-estateId" class="text-muted">Seu estado</small>
                                            </div>
                                        </div>
                                        <div class="col-md-4">
                                            <div class="form-group">
                                                <label for="city">Cidade:</label>
                                                <input type="text" name="city" id="city" data-required="true" class="form-control" placeholder="" aria-describedby="help-city">
                                                <small id="help-city" class="text-muted">Sua cidade</small>
                                            </div>
                                        </div>
                                        <div class="col-md-4">
                                            <div class="form-group">
                                                <label for="cep">Cep:</label>
                                                <input type="text" name="cep" id="cep" data-required="true" class="form-control" placeholder="" aria-describedby="help-cep">
                                                <small id="help-cep" class="text-muted">Seu cep</small>
                                            </div>
                                        </div>
                                        <div class="col-md-12">
                                            <div class="form-group">
                                                <label for="complement">Complemento:</label>
                                                <textarea type="text" name="complement" id="complement" class="form-control" placeholder="" aria-describedby="help-complement"></textarea>
                                                <small id="help-complement" class="text-muted">Informe um complemento que indentifique o local</small>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                        </div>

                        <div class="col-md-12">
                            <a class="btn btn-sm btn-warning" href="/admin/vendedor">
                                <i class="fa fa-chevron-left"></i>
                            </a>
                            <button type="submit" name="btn-salvar" id="btn-salvar" class="btn btn-sm btn-dark"><i class="fa fa-save"></i> Salvar vendedor
                            </button>
                        </div>
                    </div>
            </form>
        </div>
    </div>
</div>

<?php require_once './views/partials/scripts-admin.php' ?>
<script src="/assets/libs/jquery.mask/jquery.mask.min.js"></script>
<script src="/assets/js/models/SellerForm.js"></script>
<?php require_once './views/partials/footer-admin.php' ?>