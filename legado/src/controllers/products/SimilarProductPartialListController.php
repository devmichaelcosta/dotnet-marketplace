<?php

namespace controllers\products;

use controllers\IBaseController;
use services\SimilarProductService;

class SimilarProductPartialListController implements IBaseController
{
    private $_service;

    public function __construct($factory)
    {
        $this->_service = new SimilarProductService($factory);
    }

    public function proccessRequest(): void
    {
        $paginatedResults = $this->_service->getAllPaginated($_GET["productId"]);
        $products = $paginatedResults->results;
        require $_SERVER['DOCUMENT_ROOT'] . '\\views\\admin\\product\\list-similar-products.php';
    }
}
