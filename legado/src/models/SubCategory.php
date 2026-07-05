<?php

    namespace models;

    class SubCategory
    {
        private $_subCategoryId;
        private $_categoryId;
        private $_title;        
        private $errors;
        
        public function getTitle()
        {
            return $this->_title;
        }

        public function getSubCategoryId()
        {
            return $this->_subCategoryId;
        }

        public function getCategoryId()
        {
            return $this->_categoryId;
        }

        public function __construct($subCategoryId, $categoryId, $title)
        {
            $this->_subCategoryId = $subCategoryId;
            $this->_categoryId = $categoryId;
            $this->_title = $title;
            $this->errors = array();
        }

        public function getErrors()
        {
            return $this->errors;
        }

        public function isValid() : bool
        {
            
            if (is_null($this->getTitle()) || $this->getTitle() === "")
                $this->errors['title'] = 'Informe o titulo.';

            return count($this->errors) === 0;
        }
    }
?>