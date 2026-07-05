<?php
    namespace models;

    class SellerDetailsViewModel
    {
        private $_states; 
        
        public function __construct($seller,$products)
        {
            $this->_seller = $seller;
            $this->_products = $products;
        }
        public function getSellerId(){ return $this->_seller->getSellerId(); }
        public function getName() { return $this->_seller->getName(); }
        public function getLastName(){ return $this->_seller->getLastName(); }
        public function getFantasyName(){ return $this->_seller->getFantasyName(); }
        public function getEmail(){ return $this->_seller->getEmail(); }
        public function getWebsite(){ return $this->_seller->getWebsite(); }
        public function getProducts(){ return $this->_products; }
    }