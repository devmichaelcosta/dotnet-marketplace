<div class="row">
    <div class="col-md-6 mt-3">
        <?php
            echo "Mostrando " . $paginatedResults->qtdTotalFiltered . " de " .
            $paginatedResults->qtdTotal . " registros.";
        ?>
    </div>
    <div class="col-md-6 mt-3">
        <nav aria-label="Page navigation example">
            <ul class="pagination justify-content-end">
                <li class="page-item <?php echo $paginatedResults->attrDisablePrev; ?>">
                    <a class="page-link " 
                        href="<?php echo $paginatedResults->urlPrevPage; ?>" >Anterior</a>
                </li>
                <li class="page-item <?php echo $paginatedResults->attrDisableNext; ?>">
                    <a class="page-link" href="<?php echo $paginatedResults->urlNextPage; ?>" >Próxima</a>
                </li>
            </ul>
        </nav>
    </div>
</div>