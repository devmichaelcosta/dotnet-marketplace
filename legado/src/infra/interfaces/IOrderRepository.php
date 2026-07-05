<?php 
    
    namespace infra\interfaces;
    
    interface IOrderRepository 
    {
        public function add($order);
        public function getById($id);
        public function delete($id);
        public function total($search,$userId);
        public function getOrderWithProducts($orderId);
        public function getAll($page, $search, $pageSize, $userId);
    }

?>