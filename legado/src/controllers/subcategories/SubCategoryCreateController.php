<?php

namespace controllers\subcategories;

use controllers\IBaseController;

class SubCategoryCreateController implements IBaseController
{
    public function __construct($factory)
    {
    }

    public function proccessRequest(): void
    {
        require "views/admin/subcategories/cadastrar.php";
    }
}
