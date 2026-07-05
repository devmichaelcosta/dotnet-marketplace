<?php

namespace domain\admin;

class ProductWithSimilarProducts
{

    public function __construct($product, $paginatedResultsOfSimilarProducts)
    {
        $this->product = $product;
        $this->paginatedResultsOfSimilarProducts = $paginatedResultsOfSimilarProducts;
    }

    private $product;
    private $paginatedResultsOfSimilarProducts;

    public function getProduct()
    {
        return $this->product;
    }
    public function getPaginatedResultsOfSimilarProducts()
    {
        return $this->paginatedResultsOfSimilarProducts;
    }
}
