<?php
    namespace models;
    
    class ProductOrder
    {
        private $_orderId;
        private $_productId;
        private $_userId;
        private $_title;
        private $_seller;
        private $_qtd;
        private $_price;
        private $_subTotal;
        private $_image;

       
        public function __construct($orderId, $productId, $userId, $title, $seller, $qtd, $price, $subTotal)
        {
            $this->_orderId = $orderId;
            $this->_productId = $productId;
            $this->_userId = $userId;
            $this->_title = $title;
            $this->_seller = $seller;
            $this->_qtd = $qtd;
            $this->_price = $price;
            $this->_subTotal = $subTotal;
        }
        
        public function getProductId(){
            return $this->_productId;
        }
        public function getPrice(){
            return $this->_price;
        }
        
        public function getPriceFormatted()
        {
            return "R$ ".  number_format($this->_price,2,",",".");
        }
        public function getTitle(){
            return $this->_title;
        }
        public function getImage(){
            return $this->_image;
        }
        public function getSeller(){
            return $this->_seller;
        }
        public function getQtd(){
            return $this->_qtd;
        }
    }