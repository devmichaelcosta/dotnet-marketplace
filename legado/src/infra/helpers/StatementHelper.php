<?php

namespace infra\helpers;

use models\ProductComAssociacaoSimilar;
use models\Product;
use domain\entities\Rating;
use infra\helpers\SrcHelper;

class StatementHelper
{
    public static function ToRating($statement)
    {
        $rating = new Rating(
            $statement["RatingId"],
            $statement["ProductId"],
            $statement["Rating"],
            $statement["Recommended"],
            $statement["Title"],
            $statement["Description"],            
            $statement["UserId"],
            $statement["Approved"]
        );
        $rating->setSku($statement["Sku"]);
        $rating->setProductTitle($statement["ProductTitle"]);
        $rating->setUserName($statement["UserName"]);
        if (isset($statement["ImageDefault"]) && $statement["ImageDefault"] != "")
            $rating->setImage(SrcHelper::getProductImg() . $statement["ImageDefault"]);
        
        return $rating;
    }

    public static function ToProductComAssociacaoSimilar($productItem)
    {
        $product = new ProductComAssociacaoSimilar(
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
        $product->setAssociado($productItem["Associado"]);

        return $product;
    }

    public static function ToProduct($productItem)
    {
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
            isset($productItem["Seller"]) ? $productItem["Seller"] : ""
        );
        $product->setDefaultImage($productItem["ImageFileName"]);
        return $product;
    }
}
