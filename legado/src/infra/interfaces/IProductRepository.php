<?php

namespace infra\interfaces;

interface IProductRepository
{
    public function getAll($page, $search, $userId, $pageSize, $site);
    public function totalOfProducts($search, $userId);
    public function add($product);
    public function getCurrentStock($productId);
    public function addImages($productId, $imagesNames);
    public function update($product);
    public function decreaseStockByOrderItens($orderItens);
    public function getAllByUserIdSeller($userId);
    public function getAllSimilarProducts($productId);
    public function getAllSimilarProductsPaginated($page, $search, $userId, $pageSize, $productId);
    public function getPossibleChoicesForSimilarProducts($page, $search, $userId, $pageSize, $currentProductId);
    public function totalOfPossibleChoicesForSimilarProducts($search, $userId, $currentProductId);
    public function getAllCurrentSimilarProductsIdsByProductId($productId);
    public function addSimilarProducts($productId, $arrayOfIdsSimilarProducts);
    public function removeAllSimilarProducts($productId);
    public function deleteSimilarProduct($parentProductId, $childProductId);
    public function isLiked($productId, $userId): bool;
    public function dislike($productId, $userId);
    public function like($productId, $userId);
    public function getAllAttributesValues($productId);
    public function addAttributeValue($attributeValue);
    public function removeAllAttributesValues($productId);
    public function getAllBySubCategoryId($subCategoryId);
    public function addRating($rating);
    public function approveRating($ratingId);
    public function getAllRatingPaginated($pagina, $search, $pageSize);
    public function getAllRating($productId);
    public function getAllLikeds($userId);
    public function getAllByPreviousOrders($userId);
}
