<?php

namespace controllers;

use services\ProductService;
use infra\helpers\SrcHelper;

class SearchController implements IBaseController
{
    private $_productService;

    public function __construct($factory)
    {
        $this->_productService = new ProductService($factory);
    }

    public function proccessRequest(): void
    {
        $paginatedResults  =  $this->_productService->getAllForSearchPaginated();
        $products = $paginatedResults->results;
        $defaultSearchSrc = SrcHelper::getDefaultSearchSrc();
        require "views/home/pesquisa.php";
    }
}
