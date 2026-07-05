<?php
    namespace domain\dto;
    class SubCategoryJson
    {
        public $subCategoryId;
        public $title;
        public $categoryId;
        
        public function __construct($subCategoryId,$categoryId, $title)
        {
            $this->subCategoryId = $subCategoryId;
            $this->categoryId = $categoryId;
            $this->title = $title;
        }
    }