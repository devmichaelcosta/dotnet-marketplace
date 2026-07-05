<?php

namespace models;

use infra\helpers\SrcHelper;
use domain\catalog\viewmodels\ProductImage;

class Product
{
    private $ProductId;
    private $Title;
    private $Description;
    private $CreatedAt;
    private $CreatedBy;
    private $Price;
    private $Sku;
    private $Stock;
    private $Offer;
    private $Seller;
    private $images;
    private $imageDefault;
    private $errors;
    private $userId;
    private $imgPath;
    private $attributesValues;
    private $subCategoryId;
    private $subCategory;
    private $categoryId;
    private $subcategoryName;
    private $categoryName;
    
    public function __construct(
        $id, $title, $price, $description, $createdAt, $createdBy, $offer, $stock, $sku,
        $userId, $seller
    ) {
        $this->ProductId = $id;
        $this->Title = $title;
        $this->Price = $price;
        $this->Description = $description;
        $this->CreatedAt = $createdAt;
        $this->CreatedBy = $createdBy;
        $this->Offer = $offer;
        $this->Sku = $sku;
        $this->Stock = $stock;
        $this->userId = $userId;
        $this->Seller = $seller;
        $this->errors = array();

        $this->imgPath = SrcHelper::getProductImg();
        $this->attributesValues = array();
    }

    public function getId()
    {
        return $this->ProductId;
    }

    public function getUserId()
    {
        return $this->userId;
    }

    public function getSeller()
    {
        return $this->Seller;
    }

    public function getDescription()
    {
        return $this->Description;
    }

    public function getDescriptionFormatted()
    {
        return str_replace("\n", "<br>", $this->Description);
    }

    public function getCreatedBy()
    {
        return $this->CreatedBy;
    }

    public function getTitle()
    {
        return $this->Title;
    }

    public function getPrice()
    {
        return $this->Price;
    }

    public function getFormattedPrice()
    {
        return "R$ " .  number_format($this->Price, 2, ",", ".");
    }

    public function getStock()
    {
        return $this->Stock;
    }

    public function getOffer()
    {
        return $this->Offer;
    }

    public function getSku()
    {
        return $this->Sku;
    }

    public function setImages($images) { $this->images = $images; }
    
    public function getSubCategoryId(){ return $this->subCategoryId; }
    public function getCategoryId(){ return $this->categoryId; }
    public function getSubCategory($subCategory){ return $this->subCategory; }
    public function getSubCategoryName(){ return $this->subcategoryName; }
    public function getCategoryName(){ return $this->categoryName; }
    
    public function setSubCategoryId($subCategoryId){ $this->subCategoryId = $subCategoryId; }
    public function setCategoryId($categoryId){ $this->categoryId = $categoryId;}
    public function setSubCategory($subCategory){ $this->subCategory = $subCategory; }
    public function setSubCategoryName($subcategoryName){ $this->subcategoryName = $subcategoryName; }
    public function setCategoryName($categoryName){ $this->categoryName = $categoryName; }

    public function getImages() { return $this->images; }

    public function getImagesWithSrc()
    {
        $images = array();
        foreach ($this->images as $image) {
            $images[] = new ProductImage($image["FileName"]);
        }
        return $images;
    }

    public function hasImages()
    {
        return $this->getDefaultImage() !=  $this->imgPath;
    }

    public function setDefaultImage($img)
    {
        $this->imageDefault = SrcHelper::getProductImg() . $img;
    }

    public function getDefaultImage()
    {
        return $this->imageDefault;
    }
    
    public function getImagesStr()
    {
        $itens = array();
        foreach ($this->images as $image) {
            $itens[] = $image["FileName"];
        }
        return join("$$", $itens);
    }
    
    public function getErrors()
    {
        return $this->errors;
    }

    public function isValid(): bool
    {
        if (is_null($this->getTitle()) || $this->getTitle() === "")
            $this->errors['title'] = 'Informe o titulo.';

        if (is_null($this->getPrice()))
            $this->errors['price'] = 'Informe o preço.';

        if (is_null($this->getStock()))
            $this->errors['stock'] = 'Informe o estoque.';

        if (is_null($this->getOffer()))
            $this->errors['offer'] = 'Informe se é oferta ou não.';

        if (is_null($this->getSku()))
            $this->errors['sku'] = 'Informe o sku.';

        if (is_null($this->getDescription()) || $this->getDescription() === "")
            $this->errors['description'] = 'Informe o descritivo.';

        return count($this->errors) === 0;
    }
    
    public function addAttributeValue($attributeValue)
    {
        $this->attributesValues[] = $attributeValue;
    }

    /**
     * Get the value of attributesValues
     */ 
    public function getAttributesValues()
    {
        return $this->attributesValues;
    }

    public function addRangeAttributeValues($itens)
    {
        // echo('addRangeAttributeValues');

        // if (isset($itens) && count($itens) >0){
        //     foreach ($itens as $item) {
        //         $this->attributesValues[] = $item;
        //     }
        // }
    }
}