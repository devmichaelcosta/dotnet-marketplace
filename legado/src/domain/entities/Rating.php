<?php

namespace domain\entities;

class Rating
{
    private $productId;
    private $rating;
    private $recommended;
    private $title;
    private $description;
    private $errors;
    private $approved;
    private $userId;
    private $ratingId;
    
    private $productTitle;

    public function __construct($ratingId,$productId, $rating, $recommended, $title, $description, $userId, $approved)
    {
        $this->ratingId = $ratingId;
        $this->productId = $productId;
        $this->rating = $rating;
        $this->recommended = $recommended;
        $this->title = $title;
        $this->description = $description;
        $this->approved = $approved;
        $this->userId = $userId;
        $this->errors = array();
    }

    public function getErrors()
    {
        return $this->errors;
    }

    public function isValid(): bool
    {
        if (is_null($this->getTitle()) || $this->getTitle() === "")
            $this->errors['Title'] = 'Informe o título.';

        if (is_null($this->getDescription()) || $this->getDescription() === "")
            $this->errors['Description'] = 'Informe o descritivo.';

        return count($this->errors) === 0;
    }

    /**
     * Get the value of productId
     */ 
    public function getProductId()
    {
        return $this->productId;
    }

    /**
     * Get the value of rating
     */ 
    public function getRating()
    {
        return $this->rating;
    }

    /**
     * Get the value of recommended
     */ 
    public function getRecommended()
    {
        return $this->recommended;
    }

    /**
     * Get the value of title
     */ 
    public function getTitle()
    {
        return $this->title;
    }

    /**
     * Get the value of description
     */ 
    public function getDescription()
    {
        return $this->description;
    }

    /**
     * Get the value of userId
     */ 
    public function getUserId()
    {
        return $this->userId;
    }

    /**
     * Get the value of approved
     */ 
    public function getApproved()
    {
        return $this->approved;
    }

    public function getRatingId(){ return $this->ratingId; }

    public function getProductTitle(){ return $this->productTitle; }
    public function setProductTitle($productTitle){ $this->productTitle = $productTitle; }

    private $userName;
    public function getUserName(){ return $this->userName; }
    public function setUserName($userName){ $this->userName = $userName; }

    private $sku;
    public function getSku(){ return $this->sku; }
    public function setSku($sku){ $this->sku = $sku; }

    public function hasImage(){
        return !is_null($this->image) && isset($this->image) && $this->image != "";
    }

    private $image;
    public function getImage(){ return $this->image; }
    public function setImage($image){ $this->image = $image; }
}