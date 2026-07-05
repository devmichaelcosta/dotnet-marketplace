<div class="col-lg-5" id="img-container" data-qtd="<?php echo count($product->getImages()); ?>">
    <div class="card p-3 slider-for">
        <?php foreach ($product->getImagesWithSrc() as $image) : ?>
            <div class="text-center">
                <img src="<?php echo $image->getImage() ?>" class="img-fluid d-block" style="max-height:250px; text-align:center; display:inline !important;" alt="" title="">
            </div>
        <?php endforeach; ?>
    </div>
    <div class="slider-nav-produto">
        <?php foreach ($product->getImagesWithSrc() as $image) : ?>
            <div style="width:70px; height:70px; text-align:center">
                <img src="<?php echo $image->getImage() ?>" style="max-height:50px;display:inline;" class="img-fluid">
            </div>
        <?php endforeach; ?>
    </div>
</div>