<?php

namespace controllers;

class ProductsByCategoryController implements IBaseController
{
    private $_repoProduct;
    private $_repoCategory;
    private $_repoSubCategory;

    public function __construct($factory)
    {
        $this->_repoProduct = $factory->getProductRepository();
        $this->_repoCategory = $factory->getCategoryRepository();
        $this->_repoSubCategory = $factory->getSubCategoryRepository();
    }

    public function proccessRequest(): void
    {
        $category = $this->_repoCategory->getById($_GET["id"]);
        $subCategories = $this->_repoSubCategory->getAllByCategory($_GET["id"]);
        $category->setSubCategories($subCategories);

        require "views/produto/list-by-categories.php";
    }
}
