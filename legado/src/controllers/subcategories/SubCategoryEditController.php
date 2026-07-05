<?php

namespace controllers\subcategories;

use controllers\IBaseController;

class SubCategoryEditController implements IBaseController
{
    private $_repoSubCategory;

    public function __construct($factory)
    {
        $this->_repoSubCategory = $factory->getSubCategoryRepository();
    }

    public function proccessRequest(): void
    {
        $id = $_GET["id"];
        $category = $this->_repoSubCategory->getById($id);
        require "views/admin/subcategories/editar.php";
    }
}
