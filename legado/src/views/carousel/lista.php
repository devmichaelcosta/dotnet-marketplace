<?php if (count($caroselItens) > 0) { ?>
    <section id="secao-carrossel">
        <div id="carrossel-home" class="slick-slider">
            <?php foreach ($caroselItens as $carousel) : ?>
                <div>
                    <img src="<?php echo $carouselImgPath . $carousel["FileName"] ?>" class="d-block w-100" alt="<?php echo $carousel["FileName"] ?>" title="<?php echo $carousel["FileName"] ?>">
                </div>
            <?php endforeach ?>
        </div>
    </section>
<?php } ?>