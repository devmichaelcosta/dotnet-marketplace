<?php if ( count($categories) > 0):?> 
<section id="linhas" class="fade">
    <h3 class="text-center mt-2">Navegue pelas nossas categorias de produtos</h3>
    <div class="row">
        <div class="col-lg-12">
            <ul class="linhas-list">
                <?php foreach ($categories as $category): ?>
                <li>
                    <div class="card-linha">
                        <img src="<?php echo $category->getImage();  ?>" 
                            alt="<?php echo $category->getTitle(); ?>" class="">
                    </div>
                    <div class="card-linha-title "><?php echo $category->getTitle();  ?></div> 
                </li>
                <?php endforeach?>
            </ul>
        </div>
    </div>
</section>
<?php endif ?> 