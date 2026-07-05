<?php

namespace controllers;

class ProductsBySubCategoryController implements IBaseController
{
    private $_repoProduct;
    private $_repoSubCategory;

    public function __construct($factory)
    {
        $this->_repoProduct = $factory->getProductRepository();
        $this->_repoSubCategory = $factory->getSubCategoryRepository();
    }

    public function proccessRequest(): void
    {
        $products = $this->_repoProduct->getAllBySubCategoryId($_GET["subCategoryId"]);
        require "views/produto/list-by-subcategories.php";
    }
}