<?php 
    namespace models;

    class ProductOffer
    {
        private $productId;
        private $title;
        private $price;
        private $image;
        private $description;
        
        public function __construct($productId, $title, $price,$description,$image)
        {
            $this->productId = $productId;
            $this->title = $title;
            $this->price = $price;
            $this->description = $description;
            $this->image = $image;
        }
        
        public function getProductId()
        {
            return $this->productId;
        }
        
        public function getTitle()
        {
            return $this->title;
        }
        
        public function getPrice()
        {
            return $this->price;
        }
        
        public function getImage()
        {
            return $this->image;
        }
        public function getDescription()
        {
            return $this->description;
        }
        public function getFormattedPrice()
        {
            return number_format($this->price, 2, ",", ".");
        }
    }

?>