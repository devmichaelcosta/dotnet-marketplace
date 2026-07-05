<div class="row">
    <?php foreach ($products as $product): ?>
    <div class="col-lg-4 col-md-4 col-sm-6 mb-3 col-xs-12">
        <div class="card card-oferta fade h-100">
            <div class="p-3 text-center">
                <img src="<?php echo $product->getDefaultImage(); ?>" 
                    class="card-img-top" >
            </div>
            <div class="card-body">
                <h5 class="card-title text-center">
                <?php echo $product->getTitle(); ?>
                </h5>
                <p class="card-price h3 text-center">
                    R$ <?php echo $product->getPrice(); ?>
                </p>
                <a href="/detalhes-produto?id=<?php echo $product->getId(); ?>" 
                    class="btn btn-block btn-outline-dark"> Ver detalhes
                </a>
            </div>
        </div>
    </div>
    <?php endforeach?>
</div>