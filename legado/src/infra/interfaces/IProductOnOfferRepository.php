<?php

    namespace infra\interfaces;

    interface IProductOnOfferRepository
    {
        public function getAll();
        public function getById($id);
    }


    ?>