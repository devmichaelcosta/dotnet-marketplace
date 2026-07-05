<?php
$titlePage = "Loja Whatever - Sua loja de instrumentos músicais e acessórios";
require_once './views/partials/header.php';
?>

<!-- conteudo principal -->
<main>
    <div class="container">
        <section id="caminho-carrinho">
            <div class="row mt-3">
                <div class="col-md-12">
                    <nav aria-label="breadcrumb">
                        <ol class="breadcrumb bg-dark">
                            <li class="breadcrumb-item active" aria-current="page">Carrinho</li>
                        </ol>
                    </nav>
                </div>
            </div>
        </section>

        <section id="meu-carrinho">
            <div class="row">
                <div class="col-sm-12 col-xs-12">
                    <div class="card p-3">
                        <div class="row" id="carrinhos-itens">
                            <?php include_once './views/home/carrinho-itens.php' ?>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>
</main>

<?php require_once './views/partials/footer.php' ?>
<script src="/assets/js/models/Carrinho.js"></script>