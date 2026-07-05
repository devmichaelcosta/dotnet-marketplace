<?php

namespace services;

use models\Product;
use models\ProductCreateViewModel;
use models\ProductEditViewModel;
use infra\helpers\SrcHelper;

use domain\admin\ProductWithSimilarProducts;

class ProductService
{
    private $_repoProduct;
    private $_repoUser;
    private $_repoSubCategory;
    private $_repoCategory;
    public function __construct($factory)
    {
        $this->_repoProduct = $factory->getProductRepository();
        $this->_repoUser = $factory->getUserRepository();
        $this->_repoSubCategory = $factory->getSubCategoryRepository();
        $this->_repoCategory = $factory->getCategoryRepository();
    }

    public function getAllRatingPaginated()
    {
        $pagina = isset($_GET["p"]) ? intval($_GET["p"]) : 1 ;
        $search = isset($_GET["s"]) ? $_GET["s"] : null;
        $paginatedResults = $this->_repoProduct->getAllRatingPaginated($pagina, $search, 5, false);
        return $paginatedResults;
    }

    public function getProductCreateViewModel()
    {
        $colPrice = "col-md-6";
        $sellers = null;
        if ($_SESSION["role"] == "admin") {
            $colPrice = "col-md-3";
            $sellers = $this->_repoUser->getSellers();
        }
        $model = new ProductCreateViewModel($colPrice, $sellers);
        return $model;
    }

    public function getProductEditViewModel($productId)
    {
        $colPrice = "col-md-6";
        $sellers = null;
        if ($_SESSION["role"] == "admin") {
            $colPrice = "col-md-3";
            $sellers = $this->_repoUser->getSellers();
        }
        $categories = $this->_repoCategory->getAll();
        $model = new ProductEditViewModel($colPrice, $sellers);
        $model->setCategories($categories);
        return $model;
    }

    public function getAllForSearchPaginated()
    {
        $pagina = isset($_GET["p"]) ? intval($_GET["p"]) : 1;
        $search = isset($_GET["s"]) ? $_GET["s"] : null;
        $userId = null;

        $paginatedResults = $this->_repoProduct->getAll($pagina, $search, null, 5, true);
        $paginatedResults->results = $this->stmtToProduct($paginatedResults->results);
        return $paginatedResults;
    }

    public function getAllPaginated()
    {
        $pagina = isset($_GET["p"]) ? intval($_GET["p"]) : 1 ;
        $search = isset($_GET["s"]) ? $_GET["s"] : null;
        $userId = $_SESSION["role"] === "vendedor" ? $_SESSION["userId"] : null;

        $paginatedResults = $this->_repoProduct->getAll($pagina, $search, $userId, 5, false);
        $paginatedResults->results = $this->stmtToProduct($paginatedResults->results);
        return $paginatedResults;
    }

    public function getById($productId)
    {
        $product = $this->_repoProduct->getById($productId);
        $product->addRangeAttributeValues($this->_repoProduct->getAllAttributesValues($productId));
        $subCategory = $this->_repoSubCategory->getById(intval($product->getSubCategoryId()));        
        $product->setSubCategory($subCategory);
     
        return $product;
    }

    public function add($images, $product)
    {
        $productId = $this->_repoProduct->add($product);
        if (is_null($productId)) return null;
       
        $this->updateImages($productId, $images);
        $attributesValues = $product->getAttributesValues();
        foreach ($attributesValues as $attributeValue) {
            $attributeValue->setProductId($productId);
        }
        $this->updateAttributesValues($product);
        return $productId;
    }

    public function update($images, $product)
    {
        $productId = $product->getId();
        $this->_repoProduct->removeAllImages($productId);
        $this->_repoProduct->removeAllAttributesValues($productId);
        $this->_repoProduct->update($product);

        $this->updateImages($productId, $images);
        $this->updateAttributesValues($product);
    }

    private function updateImages($productId, $images)
    {
        if (isset($images) && !is_null($images) && $images != "") {
            $imagesNames = explode("$$", $images);
            if (count($imagesNames) > 0)
                $this->_repoProduct->addImages($productId, $imagesNames);
        }
    }

    private function updateAttributesValues($product){
        //persistindo atributos da ficha tecnica
        if (count($product->getAttributesValues()) > 0) {
            $attributesValues = $product->getAttributesValues();
            foreach ($attributesValues as $attributeValue) {
                $this->_repoProduct->addAttributeValue($attributeValue);
            }
        }
    }

    /**
     * Transforma uma lista PDO statement em uma lista de Model Product.
     *
     * @param [Array PDO Statement] $produtosResult
     * @return Product[]
     */
    public function stmtToProduct($produtosResult)
    {
        $products = array();
        foreach ($produtosResult as $productItem) {
            $product = new Product(
                $productItem["ProductId"],
                $productItem["Title"],
                number_format($productItem["Price"], 2, ",", "."),
                $productItem["Description"],
                $productItem["CreatedAt"],
                $productItem["CreatedBy"],
                $productItem["Offer"],
                $productItem["Stock"],
                $productItem["Sku"],
                $productItem["UserId"],
                $productItem["Seller"]
            );
            $product->setDefaultImage($productItem["ImageFileName"]);
            $products[] = $product;
        }

        return $products;
    }

    public function remove($productId)
    {
        $this->_repoProduct->remove($productId);
    }
}