<?php

namespace controllers\products;

use controllers\IBaseController;
use infra\repositories\ProductRepository;
use services\SimilarProductService;

class AddSimilarProductControler implements IBaseController
{
    private $_service;

    public function __construct($factory)
    {
        $this->_service = new SimilarProductService($factory);
    }

    public function proccessRequest(): void
    {
        $paginatedResults = $this->_service->getPossibleChoicesForSimilarProducts($_GET["id"]);
        $products = $paginatedResults->results;
        $currentSimilarProductsIds = $this->_service->getAllCurrentSimilarProductsIdsByProductId($_GET["id"]);
        require $_SERVER['DOCUMENT_ROOT'] . '\\views\\admin\\product\\add-similar.php';
    }
}
