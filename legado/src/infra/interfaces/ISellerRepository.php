<?php 
    
    namespace infra\interfaces;
    
    interface ISellerRepository
    {
        public function addSimplifiedSeller($userId);
        public function add($seller);
        public function update($seller);
        public function getAll($page, $search, $pageSize);
        public function total($search);
        public function remove($sellerId);
        public function getByUserId($userId);
        public function getById($sellerId);
    }

?>