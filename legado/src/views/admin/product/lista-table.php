<div class="row">
    <div class="col-md-12">
        <table id="tb-products" data-page="0" class="table table-bordered table-hovered table-striped">
            <thead>
                <tr>
                    <th width="10%"></th>
                    <th style="text-align:center">Image</th>
                    <th>Sku</th>
                    <th>Vendedor</th>
                    <th>Título</th>
                </tr>
            </thead>
            <tbody>
                <?php if (!isset($products) || count($products) == 0) : ?>
                    <tr>
                        <td colspan="5">Nenhum registro cadastrado.</td>
                    </tr>
                <?php else : ?>
                    <?php foreach ($products as $product) : ?>
                        <tr>
                            <td class="align-middle">
                                <div class="btn-group">
                                    <button class="btn btn-sm btn-outline-danger btn-delete" data-id="<?php echo $product->getId(); ?>" title="Remover produto">
                                        <i class="fa fa-remove"></i>
                                    </button>
                                    <a class="btn btn-sm btn-outline-dark" href="/admin/produto/editar?id=<?php echo $product->getId(); ?>" title="Editar produto">
                                        <i class="fa fa-edit"></i>
                                    </a>
                                    <a class="btn btn-sm btn-outline-dark" href="/admin/produto/similares?id=<?php echo $product->getId(); ?>" title="Associar produtos similares">
                                        <i class="fa fa-object-group"></i>
                                    </a>
                                </div>
                            </td>
                            <td class="text-center">
                                <?php if ($product->hasImages()) : ?>
                                    <img src="<?php echo $product->getDefaultImage(); ?>" title="" alt="" class="img-fluid" style="max-width:100px; max-height:100px; ">
                                <?php endif; ?>
                            </td>
                            <td><?php echo $product->getSku(); ?></td>
                            <td><?php echo $product->getSeller(); ?></td>
                            <td><?php echo $product->getTitle(); ?></td>
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