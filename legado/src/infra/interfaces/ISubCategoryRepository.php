<?php
    namespace infra\interfaces;
    interface ISubCategoryRepository 
    {
        public function add($category);
        public function remove($categoryId);
        public function update($category);
        public function getById($id);
        public function total($categoryId,$search);
        public function getAll();
        public function getAllPaginated($categoryId,$page, $search, $pageSize);
        public function getAllByCategory($categoryId);
    }
?>