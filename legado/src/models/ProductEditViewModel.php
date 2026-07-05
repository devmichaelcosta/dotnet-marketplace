<?php
    namespace models;

    class ProductEditViewModel extends ProductCreateViewModel
    {
        private $_categories;
         
        public function setCategories($categories)
        {
            $this->_categories = $categories;
        }

        public function getCategories(){ return $this->_categories; }
    }
?>