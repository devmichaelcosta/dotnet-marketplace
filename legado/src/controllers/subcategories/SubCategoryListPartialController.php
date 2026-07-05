<?php

namespace controllers\subcategories;

use controllers\IBaseController;

class SubCategoryListPartialController implements IBaseController
{
    private $_repoSubCategories;

    public function __construct($factory)
    {
        $this->_repoSubCategories = $factory->getSubCategoryRepository();
    }

    public function proccessRequest(): void
    {
        $page = isset($_GET["p"]) ? intval($_GET["p"]) : 1;
        $search = isset($_GET["s"]) ?  $_GET["s"] : null;
        $paginatedResults = $this->_repoSubCategories->getAllPaginated(intval($_GET["categoryId"]),$page, $search, 5);
        $subCategories  = $paginatedResults->results;
        require "views/admin/subcategories/lista-table.php";
    }
}
