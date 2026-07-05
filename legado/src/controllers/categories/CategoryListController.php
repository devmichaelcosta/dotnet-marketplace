<?php

namespace controllers\categories;

use controllers\IBaseController;

class CategoryListController  implements IBaseController
{
    private $_repoCategories;

    public function __construct($factory)
    {
        $this->_repoCategories = $factory->getCategoryRepository();
    }

    public function proccessRequest(): void
    {
        $page = 1;
        if (isset($_GET["p"]))
            $page = intval($_GET["p"]);

        $paginatedResults = $this->_repoCategories->getAllPaginated($page, null, 5);
        $categories = $paginatedResults->results;
        require "views/admin/categories/lista.php";
    }
}
