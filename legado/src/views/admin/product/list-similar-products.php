<div class="row">
    <div class="col-md-12 table-responsive">
        <table id="tb-products" data-page="0" class="table  table-bordered table-hovered table-striped">
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
                <?php  if (!isset($products) || count($products) == 0) : ?>
                    <tr>
                        <td colspan="5">Nenhum registro cadastrado.</td>
                    </tr>
                <?php else  :?>
                    <?php foreach ($products as $product): ?>
                        <tr>
                            <td>
                                <div class="btn-group">
                                    <button class="btn btn-sm btn-outline-danger btn-delete" data-id="<?php echo $product->getId(); ?>">
                                        <i class="fa fa-remove"></i>
                                    </button>
                                </div>
                            </td>
                            <td>
                                <?php if ($product->hasImages()): ?>
                                    <img src="<?php echo $product->getDefaultImage(); ?>"  title="" alt="" class="img-fluid" style="max-width:100px; max-height:100px; ">
                                <?php endif; ?>
                            </td>
                            <td >
                                <?php echo $product->getSku(); ?>
                            </td>
                            <td >
                                <?php echo $product->getSeller(); ?>
                            </td>
                            <td >
                                <?php echo $product->getTitle(); ?>
                            </td>
                        </tr>
                    <?php endforeach?>
                <?php endif ?>
            </tbody>
        </table>
    </div>
</div>