<?php 
    
    namespace infra\interfaces;
    
    interface IUserRepository 
    {
        public function add($user);
        public function remove($user);
        public function altera($user);
        public function getById($id);
        public function getByLogin($login);
        public function getAll($page, $search, $pageSize);
    }
?>