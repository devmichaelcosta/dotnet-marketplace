<?php

namespace controllers\subcategories;

use controllers\IBaseController;

class SubCategoryListController implements IBaseController
{
    private $_repoSubCategories;

    public function __construct($factory)
    {
        $this->_repoSubCategories = $factory->getSubCategoryRepository();
        $this->_repoCategories = $factory->getCategoryRepository();
    }

    public function proccessRequest(): void
    {
        $page = isset($_GET["p"]) ? intval($_GET["p"]) : 1;
        $paginatedResults = $this->_repoSubCategories->getAllPaginated(intval($_GET["id"]),$page, null, 5);
        $subCategories = $paginatedResults->results;
        $category = $this->_repoCategories->getById(intval($_GET["id"]));
        require "views/admin/subcategories/lista.php";
    }
}
