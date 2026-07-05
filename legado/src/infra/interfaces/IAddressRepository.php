<?php
    namespace infra\interfaces;

    interface IAddressRepository 
    {
        public function add($address);
        public function remove($addressId);
        public function update($address);
        public function getAllByUserId($userId);
        public function getFirstByUserId($userId);
        public function removeAllByUserId($userId);
    }
?>