<?php
    $titlePage = "Loja Whatever - Sua loja de instrumentos músicais e acessórios";
    require_once './views/partials/header.php';
    
?>
<!-- conteudo principal -->
<main>
    <div class="container">
        <?php require_once './views/produto/breadcrumb.php' ?>

        <section id="detalhes-produto">
            <div class="row">
                <div class="col-sm-12 col-xs-12">
                    <div class="card p-3">
                        <div class="row">
                            <div class="col-lg-3 text-center">
                                <img src="<?php echo $product->getDefaultImage(); ?>" class="img-fluid d-block img-sm" alt="<?php echo $product->getTitle(); ?>" title="<?php echo $product->getTitle(); ?>">
                            </div>
                            <div class="col-lg-9">
                                <h4><?php echo $product->getTitle(); ?></h4>
                                <div class="sku">(Cód. <?php echo $product->getSku(); ?>)</div>
                                <div class="vendedor">
                                    Vendido por: <strong><?php echo $product->getSeller(); ?></strong>
                                </div>
                                <div class="estoque">Qtd. estoque: <?php echo $product->getStock(); ?></div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-sm-12 col-xs-12">
                    <h4 class="mt-2">Avalie esse produto</h4>
                    <div class="card p-3">
                        <form action="/produto/avaliar-post" method="post" id="form-rating"> 
                            <input type="hidden" name="ProductId" id="ProductId" value="<?php echo $product->getId(); ?>">
                            <div>
                                <label for="">Sua avaliação para este produto*</label>
                                <div class="d-flex">
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="Rating" id="RatingRuim" value="Ruim">
                                        <label class="form-check-label" for="RatingRuim">Ruim</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="Rating" id="RatingRegular" value="Regular">
                                        <label class="form-check-label" for="RatingRegular">Regular</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="Rating" id="RatingBom" value="Bom">
                                        <label class="form-check-label" for="RatingBom">Bom</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="Rating" id="RatingOtimo" value="Ótimo">
                                        <label class="form-check-label" for="RatingOtimo">Ótimo</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="Rating" id="RatingExcelente" value="Excelente" checked="checked">
                                        <label class="form-check-label" for="RatingExcelente">Excelente</label>
                                    </div>
                                </div>
                            </div>
                            <div class="mt-4"> 
                                <label for="">Você recomenda esse produto?*</label>
                                <div class="d-flex">
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="Recommended" id="RecommendedSim" value="Sim" checked="checked">
                                        <label class="form-check-label" for="RecommendedSim">Sim</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="Recommended" id="RecommendedNao" value="Não">
                                        <label class="form-check-label" for="RecommendedNao">Não</label>
                                    </div>
                                </div>
                            </div>
                            <div class="mt-4"> 
                                <div class="form-group">
                                    <label for="Title">Título da avaliação?*</label>
                                    <input type="text" class="form-control" id="Title" name="Title" placeholder="Informe um título para a avaliação">
                                </div>
                            </div>
                            <div class="mt-2"> 
                                <div class="form-group">
                                    <label for="Description">Escreva sua opinião*</label>
                                    <textarea class="form-control" id="Description" name="Description" rows="7" maxlenght="50" placeholder="Escreva aqui sobre o produto."></textarea>
                                    <small id="Description" class="form-text text-muted">Limite de 50 caracteres.</small>
                                </div>
                            </div>
                            <button class="btn-block btn-dark btn" id="btn-rating" type="submit">Avaliar</button>
                            <small class="mt-1 form-text text-muted">* A sua avaliação pode ser publicada em nossos sites e ajudará outras pessoas na escolha de seus produtos.</small>
                        </form>
                    </div>
                </div>
            </div>
        </section>
    </div>
</main>

<?php require_once './views/partials/footer.php' ?>
<script src="/assets/js/web/product/Rating.js"></script>