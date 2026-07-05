<?php
    namespace infra;
    
    abstract class RepositoryFactory 
    {
        protected abstract function getConnection();

        public abstract function getUserRepository();
        public abstract function getStateRepository(); 
        public abstract function getSellerRepository();
        public abstract function getProductOnOfferRepository();
        public abstract function getProductRepository();
        public abstract function getSeedRepository();
        public abstract function getCartRepository();
        public abstract function getSubCategoryRepository();
        public abstract function getCategoryRepository();
        public abstract function getAttributeRepository();
    }
