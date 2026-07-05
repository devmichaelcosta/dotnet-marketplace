<?php
    namespace models;

    class ProductCreateViewModel
    {
        private $_colPrice;
        private $_sellers; 
        private $_categories;

        public function __construct($colPrice, $sellers)
        {
            $this->_colPrice = $colPrice;
            $this->_sellers = $sellers;
        }

        public function getColPrice()
        {
            return $this->_colPrice;
        }

        public function getSellers()
        {
            return $this->_sellers;
        }

    }
?>