


<div class="row">
    <div class="col-md-12">
        <table id="tb-categories" data-page="0" class="table table-bordered table-hovered table-striped">
            <thead>
                <tr>
                    <th width="10%"></th>
                    <th>Nome</th>
                </tr>
            </thead>
            <tbody>
                <?php  if (count($categories) == 0) : ?>
                    <tr>
                        <td colspan="2">Nenhum registro cadastrado.</td>
                    </tr>
                <?php else  :?>
                    <?php foreach ($categories as $category): ?>
                        <tr>
                            <td>
                                <div class="btn-group">
                                    <button class='btn btn-sm btn-outline-danger btn-delete' 
                                        data-id="<?php echo $category->getCategoryId(); ?>">
                                        <i class="fa fa-remove"></i>
                                    </button>
                                    <a class='btn btn-sm btn-outline-dark' 
                                        href="/admin/categoria/editar?id=<?php echo $category->getCategoryId(); ?>">
                                        <i class="fa fa-edit" ></i>
                                    </a>
                                </div>
                            </td>
                            <td>
                                <a href="/admin/subcategoria?id=<?php echo $category->getCategoryId(); ?>"><?php echo $category->getTitle(); ?></a>
                            </td>
                        </tr>
                    <?php endforeach?>
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