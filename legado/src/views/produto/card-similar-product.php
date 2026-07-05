<?php if (count($similarProducts) > 0) :?>
<section>
    <h3 class="mt-3">Produtos semelhantes </h5>
    <div class="row ">
        <div class="col-md-12 similar-products-slider">
            <?php  foreach($similarProducts as $similar) :?>
                <div class="card card-oferta fade in h-100 mr-2">
                    <div class="text-center p-2">
                        <img src="<?php echo $similar->getDefaultImage() ?>" 
                            class="card-img-top img-fluid" style="display:inline;"
                            alt="<?php echo $similar->getDefaultImage() ?>" 
                            title="<?php echo $similar->getDefaultImage() ?>">
                    </div>
                    <div class="card-body">
                        <h5 class="card-title text-center">
                            <?php echo $similar->getTitle() ?>
                        </h5>
                        <p class="card-price h3 text-center">
                            <?php echo $similar->getFormattedPrice() ?>                            
                        </p>
                        <a href="/detalhes-produto?id=<?php echo $similar->getId() ?>" class="btn btn-block btn-outline-dark"> 
                            Ver detalhes
                        </a>
                    </div>
                </div>
            <?php endforeach; ?>
        </div>
    </div>
</section>
<?php endif; ?>