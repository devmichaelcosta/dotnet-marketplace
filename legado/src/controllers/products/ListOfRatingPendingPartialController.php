<?php

namespace controllers\products;

use controllers\IBaseController;
use services\ProductService;

class ListOfRatingPendingPartialController implements IBaseController
{
    private $_productService;

    public function __construct($factory)
    {
        $this->_productService = new ProductService($factory);
    }

    public function proccessRequest(): void
    {
        $paginatedResults = $this->_productService->getAllRatingPaginated();
        $ratings = $paginatedResults->results;
        require $_SERVER['DOCUMENT_ROOT'] . '\\views\\admin\\product\\list-rating-pending.php';
    }
}
