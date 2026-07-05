<div class="row">
    <div class="col-md-12">
        <table id="tb-products" data-page="0" class="table table-bordered table-hovered table-striped">
            <thead>
                <tr>
                    <th style="width:20px;"></th>
                    <th style="text-align:center">Image</th>
                    <th>Avaliação</th>
                </tr>
            </thead>
            <tbody>
                <?php if (!isset($ratings) || count($ratings) == 0) : ?>
                    <tr>
                        <td colspan="3">Nenhum registro cadastrado.</td>
                    </tr>
                <?php else : ?>
                    <?php foreach ($ratings as $rating) : ?>
                        <tr>
                            <td class="align-middle">
                                <div class="btn-group">
                                    <button class="btn btn-sm btn-outline-success btn-approve" data-id="<?php echo $rating->getRatingId(); ?>" 
                                        title="Aprovar avaliação de produto">
                                        <i class="fa fa-check"></i>
                                    </button>
                                </div>
                            </td>
                            <td class="text-center">
                                <?php if ($rating->hasImage()) : ?>
                                    <img src="<?php echo $rating->getImage(); ?>" title="" alt="" class="img-fluid" 
                                    style="max-width:100px; max-height:100px; ">
                                <?php endif; ?>
                            </td>
                            <td>
                                <strong>Sku: </strong><?php echo $rating->getSku(); ?><br />
                                <strong>Produto:</strong><?php echo $rating->getProductTitle(); ?><br />
                                <strong>Título:</strong><?php echo $rating->getTitle(); ?><br />
                                <strong>Avaliação:</strong><?php echo $rating->getRating(); ?><br />
                                <strong>Recomenda:</strong><?php echo ($rating->getRecommended() == 1 ? "Sim" : "Não"); ?><br /> 
                                <strong>Descrição:</strong><br />
                                <?php echo $rating->getDescription(); ?>
                            </td>
                        </tr>
                    <?php endforeach ?>
                <?php endif ?>
            </tbody>
        </table>
    </div>
</div>
<div class="row">
    <div class="col-md-12">
        <?php require_once './views/partials/pagination-admin-controlls.php' ?>
    </div>
</div>