<?php

namespace controllers\products;

use controllers\IBaseController;
use infra\helpers\SrcHelper;
use services\SimilarProductService;

class ProductSimilarController implements IBaseController
{
    private $_productService;

    public function __construct($factory)
    {
        $this->_productService = new SimilarProductService($factory);
    }

    public function proccessRequest(): void
    {
        $productWithSimilarProducts = $this->_productService
            ->getProductByIdWithSimilarProducts($_GET['id'], 0, null, 10);

        $pathJs = SrcHelper::getProductAdminJs();
        $model = $productWithSimilarProducts->getProduct();
        $paginatedResults = $productWithSimilarProducts->getPaginatedResultsOfSimilarProducts();
        $products = $productWithSimilarProducts->getPaginatedResultsOfSimilarProducts()->results;

        require $_SERVER['DOCUMENT_ROOT'] . '\\views\\admin\\product\\similar.php';
    }
}
