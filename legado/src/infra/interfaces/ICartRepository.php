<?php 
    
    namespace infra\interfaces;
    
    interface ICartRepository 
    {
        public function addProduct($productId, $cartGroup);
        public function getQtdProduct($productId, $cartGroup);
        public function getProducts($cartGroup);
        public function getFinalPrice($cartGroup);
    }

?>