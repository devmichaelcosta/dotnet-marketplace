<?php

namespace controllers\categories;

use controllers\IBaseController;

class CategoryListPartialController implements IBaseController
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

        $search = null;
        if (isset($_POST["s"]))
            $search = $_POST["s"];

        $paginatedResults = $this->_repoCategories->getAllPaginated($page, $search, 5);
        $categories = $paginatedResults->results;
        require "views/admin/categories/lista-table.php";
    }
}
