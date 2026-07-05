<div class="row">
    <div class="col-md-12">
        <table id="tb-attributes" data-page="0" class="table table-bordered table-hovered table-striped">
            <thead>
                <tr>
                    <th width="10%"></th>
                    <th>Nome</th>
                </tr>
            </thead>
            <tbody>
                <?php if (count($attributes) == 0) : ?>
                    <tr>
                        <td colspan="2">Nenhum registro cadastrado.</td>
                    </tr>
                <?php else : ?>
                    <?php foreach ($attributes as $attribute) : ?>
                        <tr>
                            <td>
                                <div class="btn-group">
                                    <button class='btn btn-sm btn-outline-danger btn-delete' data-id="<?php echo $attribute->getAttributeId(); ?>">
                                        <i class="fa fa-remove"></i>
                                    </button>
                                    <a class='btn btn-sm btn-outline-dark' href="/admin/atributo/editar?id=<?php echo $attribute->getAttributeId(); ?>">
                                        <i class="fa fa-edit"></i>
                                    </a>
                                </div>
                            </td>
                            <td><?php echo $attribute->getName(); ?></td>
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