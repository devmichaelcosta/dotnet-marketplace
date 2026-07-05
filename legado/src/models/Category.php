<?php

    namespace models;

    class Category
    {
        public $_categoryId;
        public $_title;        
        public $_image;
        public $_subCategories;

        private $errors;
        
        public function getTitle()
        {
            return $this->_title;
        }
        
        public function getCategoryId()
        {
            return $this->_categoryId;
        }
        
        public function getImage()
        {
            return $this->_image;
        }

        public function getSubCategories()
        {
            return $this->_subCategories;
        }

        public function setSubCategories($subCategories){
            $this->_subCategories = $subCategories;
        }

        public function setImage($image)
        {
            $this->_image = $image;
        }

        public function __construct($categoryId, $title, $image)
        {
            $this->_categoryId = $categoryId;
            $this->_title = $title;
            $this->_image = $image;
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