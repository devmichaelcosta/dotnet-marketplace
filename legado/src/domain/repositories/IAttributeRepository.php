<?php
    namespace domain\repositories;

    interface IAttributeRepository 
    {
        public function add($attribute);
        public function remove($atributeId);
        public function update($attribute);
        public function getById($atributeId);
        public function total($search);
        public function getAll();
        public function getAllPaginated($page, $search, $pageSize);
    }
