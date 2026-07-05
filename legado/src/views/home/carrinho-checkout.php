<?php
$titlePage = "Loja Whatever - Sua loja de instrumentos músicais e acessórios";
require_once './views/partials/header.php';
?>

<!-- conteudo principal -->
<main>
    <div class="container">
        <!-- breadcrumb -->
        <section id="caminho-carrinho">
            <div class="row mt-3">
                <div class="col-md-12">
                    <nav aria-label="breadcrumb">
                        <ol class="breadcrumb bg-dark text-white">
                            <li class="breadcrumb-item " aria-current="page">Meu carrinho</li>
                            <li class="breadcrumb-item " aria-current="page">Identificação</li>
                            <li class="breadcrumb-item active" aria-current="page">Pagamento</li>
                        </ol>
                    </nav>
                </div>
            </div>
        </section>

        <section id="meu-carrinho">
            <form action="cart-checkout-post" method="post" id="form-cadastro">
                <div class="row">
                    <div class="col-sm-12 col-xs-12">
                        <div class="card p-3">
                            <div class="row">
                                <div class="col-lg-8">
                                    <h5>Para prosseguir com a compra informe os dados abaixo: </h5>
                                </div>
                                <div class="col-lg-4">
                                    <h5>Resumo da compra: </h5>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-8">
                                    <div class="category-fieldset">
                                        <span class="title">Dados pessoais:</span>
                                        <div class="row">
                                            <div class="col-lg-7">
                                                <div class="form-group">
                                                    <label for="">Nome:</label>
                                                    <input type="text" data-required="true" class="form-control" name="name" id="name" aria-describedby="nameId" placeholder="">
                                                    <small id="nameId" class="form-text text-muted">
                                                        Seu nome.
                                                    </small>
                                                </div>
                                            </div>
                                            <div class="col-lg-5">
                                                <div class="form-group">
                                                    <label for="">Data Nascimento:</label>
                                                    <input type="date" data-required="true" class="form-control" name="dateOfBirth" id="dateOfBirth" aria-describedby="dateOfBirthHelp" placeholder="">
                                                    <small id="dateOfBirthHelp" class="form-text text-muted">
                                                        Sua data de dascimento
                                                    </small>
                                                </div>
                                            </div>
                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label for="">Sobrenome:</label>
                                                    <input type="text" data-required="true" class="form-control" name="lastName" id="lastName" aria-describedby="cpfId" placeholder="">
                                                    <small id="lastNameHelp" class="form-text text-muted">
                                                        Seu sobrenome.
                                                    </small>
                                                </div>
                                            </div>
                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label for="">Cpf:</label>
                                                    <input type="text" data-required="true" class="form-control" name="cpf" id="cpf" aria-describedby="cpfId" placeholder="">
                                                    <small id="cpfId" class="form-text text-muted">
                                                        Seu cpf.
                                                    </small>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="category-fieldset">
                                        <span class="title">Dados de pagamento:</span>
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <div class="row">
                                                    <div class="col-lg-6">
                                                        <div class="form-group">
                                                            <label for="card-number">N. Cartão</label>
                                                            <input id="card-number" data-required="true" maxlength="16" class="form-control" type="text" name="card-number">
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-6">
                                                        <div class="form-group">
                                                            <label for="card-name">Nome impresso</label>
                                                            <input id="card-name" data-required="true" class="form-control" type="text" name="card-name">
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-6">
                                                        <div class="form-group">
                                                            <label for="mes">Data de Expiração</label>
                                                            <input type="text" data-required="true" id="card-expiration" name="card-expiration" class="form-control">
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <div class="form-group">
                                                            <label for="cvv">CVV</label>
                                                            <input id="cvv" data-required="true" class="form-control" type="text" name="cvv">
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="category-fieldset">
                                        <span class="title">Dados de entrega:</span>
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <div class="form-group">
                                                    <label for="">Endereço:</label><?php echo $model->getStreet(); ?>
                                                    <input type="text" data-required="true" class="form-control" name="street" id="street" value="<?php echo $model->getStreet(); ?>" aria-describedby="streetId" placeholder="">
                                                    <small id="streetId" class="form-text text-muted">
                                                        Sua rua, seu número de apto.
                                                    </small>
                                                </div>
                                            </div>
                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label for="">CEP:</label>
                                                    <input type="text" data-required="true" class="form-control" name="cep" id="cep" value="<?php echo $model->getCep(); ?>" aria-describedby="cepId" placeholder="">
                                                    <small id="cepId" class="form-text text-muted">
                                                        Seu cep.
                                                    </small>
                                                </div>
                                            </div>
                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label for="">Bairro:</label>
                                                    <input type="text" data-required="true" class="form-control" name="neighborhood" id="neighborhood" value="<?php echo $model->getNeighborhood(); ?>" aria-describedby="neighborhoodId" placeholder="">
                                                    <small id="neighborhoodId" class="form-text text-muted">Seu bairro</small>
                                                </div>
                                            </div>
                                            <div class="col-lg-4">
                                                <div class="form-group">
                                                    <label for="">Estado:</label>
                                                    <select data-required="true" class="form-control" name="stateId" id="stateId" aria-describedby="stateId" placeholder="">
                                                        <option value="">Selecione</option>
                                                        <?php foreach ($model->getStates() as $state) : ?>
                                                            <option value="<?php echo $state->getId(); ?>" <?php echo ($model->getStateId() == $state->getId() ? "selected=selected" : ""); ?>>
                                                                <?php echo $state->getName(); ?>
                                                            </option>
                                                        <?php endforeach; ?>
                                                    </select>
                                                    <small id="ufId" class="form-text text-muted">
                                                        Seu estado
                                                    </small>
                                                </div>
                                            </div>
                                            <div class="col-lg-8">
                                                <div class="form-group">
                                                    <label for="">Cidade:</label>
                                                    <input type="text" data-required="true" class="form-control" name="city" id="city" value="<?php echo $model->getCity(); ?>" aria-describedby="cityId" placeholder="">
                                                    <small id="cityId" class="form-text text-muted">
                                                        Sua cidade
                                                    </small>
                                                </div>
                                            </div>
                                            <div class="col-lg-12">
                                                <div class="form-group">
                                                    <label for="">Complemento:</label>
                                                    <textarea type="text" data-required="true" class="form-control" id="complement" name="complement" aria-describedby="complement"><?php echo $model->getComplement(); ?></textarea>

                                                    <small id="complementId" class="form-text text-muted">
                                                        Complemento
                                                    </small>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <!-- resumo da compra -->
                                <div class="col-md-4">
                                    <div class="list-group list-itens-cart-checkout">
                                        <?php foreach ($model->getProducts() as $product) : ?>
                                            <a href="#" class="list-group-item list-group-item-action bg-default ">
                                                <?php echo $product->getTitle(); ?>
                                            </a>
                                        <?php endforeach; ?>

                                        <a href="#" class="list-group-item total list-group-item-action bg-success text-white " tabindex="-1" aria-disabled="true">
                                            Total: <?php echo $model->getTotal(); ?>
                                        </a>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-12">
                                    <button class="btn btn-block btn-success" title="Finalizar compra" id="btn-save" type="submit" data-loading-text="Processando, aguarde...">
                                        Finalizar compra</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </form>
        </section>
    </div>
</main>

<?php require_once './views/partials/footer.php' ?>

<script src="/assets/libs/jquery.mask/jquery.mask.min.js"></script>
<script src="/assets/js/models/GenericValidator.js"></script>
<script src="/assets/js/models/CartCheckout.js"></script>