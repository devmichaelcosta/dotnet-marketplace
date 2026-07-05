<?php

namespace controllers\categories;

use controllers\IBaseController;

class CategoryCreateController implements IBaseController
{
    public function __construct($factory)
    {
    }

    public function proccessRequest(): void
    {
        require "views/admin/categories/cadastrar.php";
    }
}
