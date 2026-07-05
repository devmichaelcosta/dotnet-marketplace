<?php require_once './views/partials/header-admin.php' ?>
<div class="container">
    <div class="d-flex align-items-center p-3 mt-3 text-white-50 bg-dark rounded shadow-sm">
        <div class="lh-100">
            <h6 class="mb-0 text-white lh-100">Detalhes do pedido</h6>
            <ul class="nav-breadcrumb">
                <li>
                    <a href="/admin/usuario/minhas-compras">Minhas compras</a>
                </li>
                <li>
                    <a href="/admin/pedido/detalhes?id=<?php echo $_GET["id"]; ?>">Detalhe do pedido</a>
                </li>
            </ul>
        </div>
    </div>

    <div class="card mt-3">
        <div class="card-body">
            <h5>Veja abaixo detalhes do pedido <strong><?php echo $order->getOrderId(); ?></strong>.</h5>
            <small>Pedido criado em: <?php echo $order->getCreateAtFormatted(); ?></small>
            <div class="row">
                <div class="col-md-5 order-md-2 mb-4">
                    <h4 class="d-flex justify-content-between align-items-center mb-3">
                        <span class="text-muted">
                            Produtos do pedido
                        </span>
                        <span class="badge badge-secondary badge-pill">
                            <?php echo count($order->getProducts()); ?>
                        </span>
                    </h4>
                    <ul class="list-group mb-3">
                        <?php foreach($order->getProducts() as $product):?>
                        <li class="list-group-item ">
                            <div class="row">
                                <div class="col-md-8">
                                    <h6 class="my-0"><small><?php echo $product->getTitle(); ?></small></h6>
                                    <small class="text-muted"></small>
                                </div>
                                <span class="col-md-4 text-muted text-right">
                                    <?php echo $product->getQtd(); ?>X   
                                    <?php echo $product->getPriceFormatted(); ?>
                                </span>
                            </div>
                        </li>
                        <?php endforeach; ?>
                        <li class="list-group-item ">
                            <div class="row">
                                <div class="col-md-8">Total (R$)</div>
                                <div class="col-md-4 text-right">
                                <strong><?php echo $order->getTotalFormatted(); ?></strong>
                                </div>
                            </div>
                        </li>
                    </ul>
                </div>
                
                <div class="col-md-7 order-md-1">
                    <div class="address-container mb-3">
                        <h5 class="mb-1">Cliente:</h5>
                        <div class="row mb-3 ">
                            <div class="col-md-12 mb-12">
                                <strong class="text-gray-dark">Nome: </strong>
                                <?php echo $order->getName();?>
                            </div>
                            <div class="col-md-12 mb-12">
                                <strong class="text-gray-dark">CPF: </strong>
                                <?php echo $order->getCpf();?>
                            </div>
                        </div>
                    </div>
                    <div class="address-container">
                        <h5 class="mb-1">Dados de entrega</h5>                
                        <div class="row">
                            <div class="col-md-7 ">
                                <strong class="text-gray-dark">Rua: </strong>
                                <?php echo $order->getAddress();?>
                            </div>
                            <div class="col-md-5">
                                <strong class="text-gray-dark">Bairro: </strong>
                                <?php echo $order->getNeighborhood();?>
                            </div>
                            <div class="col-md-7">
                                <strong class="text-gray-dark">Cep: </strong>
                                <?php echo $order->getCep();?>
                            </div>
                            <div class="col-md-5">
                                <strong class="text-gray-dark">Cidade: </strong>
                                <?php echo $order->getCity();?>
                            </div>
                            <div class="col-md-12">
                                <strong class="text-gray-dark">Estado: </strong>
                                <?php echo $order->getState();?>
                            </div>
                            <div class="col-md-12">
                                <strong class="text-gray-dark">Complemento: </strong><br>
                                <?php echo $order->getComplement();?>
                            </div>
                        </div>
                    </div>
                  
                </div>
            </div>
            <div class="row mt-3">
                <div class="col-md-12">
                    <a class="btn btn-warning" href="/admin/usuario/minhas-compras"><i class="fa fa-chevron-left"></i></a>
                </div>
            </div>
        </div>
    </div>
</div>

<?php require_once './views/partials/scripts-admin.php' ?>

<?php require_once './views/partials/footer-admin.php' ?>