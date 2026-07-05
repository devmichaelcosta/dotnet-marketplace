<section>
    <h3 class="mt-3">Avaliações</h3>
    <div class="row">
        <div class="col-sm-12 col-xs-12">
            <div class="card p-3">
                <?php if(isset($_SESSION["userId"])):?>
                <div class="row">
                    <div class="col-lg-10 d-flex">
                        <div class="align-self-center mb-0 w-100">
                            <?php if(count($ratings) == 0):?>
                            <div class="p-t-2 d-block text-center">Seja o primeiro a avaliar</div>
                            <?php endif;?>
                        </div>
                    </div>
                    <div class="col-lg-2">
                        <a class="btn-outline-dark btn pull-right" href="/produto/avaliar?id=<?php echo $product->getId(); ?>">Avaliar produto</a>
                    </div>
                </div>
                <?php endif;?>


                <?php if(count($ratings) > 0):?>
                    <ul class="list-group list-group-flush mt-5">
                    <?php foreach ($ratings as $rating): ?>
                        <li class="list-group-item">
                            <div class="row">
                                <div class="col-md-12">
                                    <h4><?php echo $rating->getTitle();?></h4>
                                    <div>
                                        <p><?php echo $rating->getDescription();?></p>
                                        <span class="badge badge-dark">Por: <?php echo $rating->getUserName();?></span>                            
                                    </div>
                                </div>
                            </div>
                        </li>
                    <?php endforeach;?>
                    </ul>
                <?php endif;?>
            </div>
        </div>
    </div>
</section>