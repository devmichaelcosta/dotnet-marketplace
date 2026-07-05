<?php if($cartViewModel != null && count($cartViewModel->getProducts()) > 0):?>
    <!--LISTA DE ITENS DO PEDIDO-->
    <div class="col-lg-8">
        <h4>Meu Carrinho</h4>
        
        <table class="table table-condensed cols-centered table-cart">
            <thead>
                <tr>
                    <th width="10px"></th>
                    <th>Img</th>
                    <th>Produto</th>
                    <th width="100px">Qtd.</th>
                    <th width="100px">Preço</th>
                </tr>
            </thead>
            <tbody>
                <?php foreach($cartViewModel->getProducts() as $productArray ) : ?>
                    <tr>
                        <td>
                            <button class="btn-danger btn btn-sm btn-delete" 
                                title="Remover item do carrinho" 
                                data-id="<?php echo $productArray->getProductId(); ?>">
                                <i class="fa fa-trash"></i>
                            </button>
                        </td>
                        <td class="center img ">
                            <a href="/detalhes-produto?id=<?php echo $productArray->getProductId(); ?>">
                            <img src="<?php echo $productArray->getImage(); ?>"  
                                alt="<?php echo $productArray->getImage(); ?>" /></a>
                        </td>
                        <td class="center title">
                            <?php echo $productArray->getTitle(); ?>
                        </td>
                        <td>
                            <input type="text" min="1" data-product-id="<?php echo $productArray->getProductId(); ?>"
                                value="<?php echo $productArray->getQtd(); ?>" 
                                class="form-control qtd-product">
                        </td>
                        <td class="text-center product-price" 
                            data-product-id="<?php echo $productArray->getProductId(); ?>">
                            <?php echo  $productArray->getSubTotalFormatted(); ?>
                        </td>
                    </tr>
                <?php endforeach ?>   
                    
            </tbody>
        </table>
    
    </div>

    <!--RESUMO DO PEDIDO-->
    <div class="col-lg-4">
        <div class="card h-100 bg-light p-2 resumo-carrinho">
            <h4>Resumo do pedido</h4>
            <div class="row">
                <div class="col-lg-7">
                    Subtotal (<?php echo $cartViewModel->getQtdProducts();?> produtos)
                </div>
                <div id="cart-sub-total" class="col-lg-5 text-right" >
                    <?php echo $cartViewModel->getSubTotalFormatted(); ?>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-6">
                    Frete
                </div>
                <div class="col-lg-6 text-right">
                    <?php echo $cartViewModel->getFreteValor(); ?>
                </div>
            </div>
            <hr/>    
            <div class="row">
                <div class="col-lg-6">
                    Total
                </div>
                <div class="col-lg-6 text-right" id="cart-total">
                    <?php echo $cartViewModel->getTotalFormatted(); ?>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">Em até 1x s/ juros</div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <a class="btn btn-block btn-success" href="/finalizar-pedido">
                        <i class="fa fa-check"></i> Finalizar pedido
                    </a>
                </div>
            </div>
        </div>

    </div>

<?php else : ?>  
    <div class="col-lg-12">
        <h4>Meu Carrinho</h4>      
        <div class="alert alert-info" role="alert">
        Não há produtos no carrinho.
        </div>
    </div>
<?php endif ;?>  