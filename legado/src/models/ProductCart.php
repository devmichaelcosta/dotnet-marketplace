<?php
    namespace models;
    
    use models\helpers\PathHelper;

    class ProductCart
    {
        private $CartId;
        private $CartGroup;
        private $ProductId;
        private $Title;       
        private $Qtd;
        private $Price;
        private $Image;
        private $helper;
        private $subTotal;
        
        public function __construct($cartId, $cartGroup, $productId, $title, $price, $qtd, $image,$subTotal)
        {
            $this->CartId = $cartId;
            $this->CartGroup = $cartGroup;
            $this->ProductId = $productId;
            $this->Title = $title;
            $this->Price = $price;
            $this->Qtd = $qtd;
            $this->Image = $image;
            $this->subTotal = $subTotal;
            $this->helper = new PathHelper();
        }

        public function getProductId(){
            return $this->ProductId;
        }

        public function getCartId(){
            return $this->CartId;
        }

        public function getCartGroup(){
            return $this->CartGroup;
        }

        public function getTitle(){
            return $this->Title;
        }
        
        public function getPrice(){
            return $this->Price;
        }

        public function getQtd()
        {
            return $this->Qtd;
        }

        public function setQtd($qtd)
        {
            $this->Qtd = $qtd;
        }
        public function calcValue(){
            return $this->Qtd * $this->Price;
        }
        public function incrementQtd()
        {
            $this->Qtd = $this->Qtd + 1;
        }

        public function getImage()
        {
            return $this->helper->getPathImgProduct() . $this->Image;
        }

        public function getSubTotal()
        {
            return $this->subTotal * $this->Qtd;
        }
        public function getSubTotalFormatted()
        {
            return "R$ ".  number_format($this->getSubTotal(),2,",",".");
        }
    }
?>