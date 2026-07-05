<?php
namespace domain\dto;
class CategoryJson
{
    public $categoryId;
    public $title;
    
    public function __construct($categoryId,$title)
    {
        $this->categoryId = $categoryId;
        $this->title = $title;
    }
}
